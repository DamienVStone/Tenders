using AppLogger.Configuration;
using AppLogger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AppLogger
{
    public class Logger
    {
        public static int BatchSize = 10000;
        public static int MaxAttemptsCount = 3;

        private static readonly object key = new object();

        private static Dictionary<string, LoggerConfig> credentials = null;
        private static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

        public static Action<string> LogCallback = null;

        private static string _apiUrl;
        private static long _httpTimeout;
        private static long _statusTimeout;
        private static long _logTimeout;

        private static LogType _minLogLevel;
        
        static Logger()
        {
            var section = (SSDSApiLoggerSection)ConfigurationManager.GetSection("SSDSApiLogger");
            if (section == null) throw new InvalidOperationException("Не могу найти секцию SSDSApiLogger в файле конфигурации");

            _apiUrl = section.ApiUrl;
            LogCallback?.Invoke($"INFO [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] LIBRARY: Точка входа для логгера: '{_apiUrl}'");

            _minLogLevel = section.LogLevel;
            LogCallback?.Invoke($"INFO [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] LIBRARY: Минимальный уровень логгирования: '{_minLogLevel}'");

            _httpTimeout = section.Timeout?.Http ?? 3000L;
            _statusTimeout = section.Timeout?.Status ?? 1000L;
            _logTimeout = section.Timeout?.Log ?? 1000L;

            LogCallback?.Invoke($"INFO [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] LIBRARY: Таймауты (мс): HTTP - '{_httpTimeout}', Статусы - '{_statusTimeout}', Логи - '{_logTimeout}'");

            credentials = new Dictionary<string, LoggerConfig>();
            new List<LoggerConfig>(section.Loggers).ForEach(l => credentials[l.Username] = l);
        }

        public static Logger GetLogger(string name)
        {
            lock (key)
            {
                if (loggers.ContainsKey(name))
                {
                    return loggers[name];
                }
                
                if (!credentials.ContainsKey(name)) throw new ArgumentException("Не могу найти конфигурацию для логгера с именем: " + name);
                var config = credentials[name];

                loggers[name] = new Logger(config.Username, config.Password);
                return loggers[name];
            }
        }

        //------------------------------------------------------------- далее только нестатическое содержимое
        
        private Token _token = null;
        private DateTime _expirationDate;
        private Dumper _dumper;

        private string _username;
        private string _password;

        private LogCache _logs;
        private StatusCache _statuses;

        private bool _isWaiting = false;
        public void Wait()
        {
            _isWaiting = true;
            while (_isWaiting)
            {
                Thread.Sleep(500);
            }
        }

        private Logger(string Username, string Password)
        {
            _username = Username;
            _password = Password;

            setupCaches();
            _dumper = new Dumper(_username);
        }

        public Task LogAsync(LogType LogType, string Message)
        {
            if (LogType == LogType.DEFAULT) throw new ArgumentException();
            if (LogType < _minLogLevel) return Task.CompletedTask;
            return Task.Run(() => _logs.Add(new LogEntry() { Id = _username, Message = Message, Timestamp = DateTime.Now, LogType = LogType }));   
        }

        public Task SetStatusAsync(string StatusKey, string StatusValue)
        {
            return Task.Run(() => _statuses.Add(StatusKey, new StatusEntry() { Key = StatusKey, Value = StatusValue }));
        }

        private void setupCaches()
        {
            _logs = new LogCache(logs => {
                if (_isWaiting && !logs.Any())
                {
                    _isWaiting = false;
                }

                logs.ForEach(strContent =>
                {
                    //var content = new StringContent(strContent);
                    //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    try
                    {
                        POSTAuthorized("/api/log", strContent);

                    }
                    catch (Exception exp)
                    {
                        LogException(exp);
                        try
                        {
                            LogString("Сохраняю дамп логов");
                            _dumper.DumpLogs(strContent);
                        }
                        catch (Exception)
                        {
                            LogString("Не удалось сохранить дамп логов", prefix: "FATAL");
                            LogException(exp);
                            LogString("Потерянные логи: " + strContent, prefix: "FATAL");
                        }
                        return;//не пытаемся восстановить данные из дампа если сами только что что-то писали в дамп. просто выходим из метода.
                    }
                    _dumper.retriveLogs((i, log) =>
                    {
                        LogString($"Восстанавливаю данные логов из дампа и отправляю на сервер часть: {i}");
                        //var cont = new StringContent(log);
                        //cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        POSTAuthorized("/api/log", log);
                    });
                });
            }, _logTimeout);


            _statuses = new StatusCache(statuses =>
            {
                statuses.ForEach(serializedData =>
                {
                    //var content = new StringContent(serializedData);
                    //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var url = "/api/status";
                    try
                    {
                        POSTAuthorized(url, serializedData);
                    }
                    catch (Exception exp)
                    {
                        //Многие ошибки перехватываются выше, сюда мы попадем только когда все совсем плохо.
                        LogException(exp);
                        LogString("Операция прервана.", prefix: "FATAL");
                        try
                        {
                            LogString("Сохраняю дамп статуса");
                            _dumper.DumpStatus(serializedData);
                        }
                        catch (Exception ex)
                        {
                            LogString("Не удалось сохранить дампы статуса", prefix: "FATAL");
                            LogException(ex);
                            LogString($"Потеряные статусы: {serializedData}");
                        }
                        return;//не пытаемся восстановить данные из дампа если сами только что что-то писали в дамп. просто выходим из метода.
                    }
                    //если какой-то дамп есть, он будет восстановлен, отправлен на сервер, а файл удален с диска.
                    _dumper.retriveStatus((i, stat) =>
                    {
                        LogString($"Восстанавливаю данные статусов из дампа и отправляю на сервер часть: {i}");
                        //var cont = new StringContent(stat);
                        //cont.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        POSTAuthorized(url, stat);
                    });
                });
            }, _statusTimeout);
        }

        private void POSTAuthorized(string path, string content)
        {
            for (int i = 0; i < 2; i++)
            {
                if (_token == null || _expirationDate == null || _expirationDate <= DateTime.Now)
                {
                    RenewToken();
                }
                try
                {   
                    POST(path, content);
                    return;
                }
                catch (UnauthorizedException)
                {
                    RenewToken();
                }
                catch (ForbbidenException)
                {
                    RenewToken();
                }
            }
            LogString("Не удается повторно авторизоваться!", prefix: "FATAL");
            throw new MaxAttemptsException(_username);
        }

        private void RenewToken()
        {
            LogString("Обновляю токен");
            var createDate = DateTime.Now;

            var result = POST("/Token", $"grant_type=password&username={_username}&password={_password}", ShouldAuthorize: false);

            var body = result.Content.ReadAsStringAsync().Result;
            _token = new JavaScriptSerializer().Deserialize<Token>(body);
            _expirationDate = createDate.AddSeconds(_token.expires_in);
        }

        private HttpResponseMessage POST(string path, string serializedJSON, bool ShouldAuthorize = true)
        {
            var id = Guid.NewGuid();
            LogString($"Отправляю запрос {id} на {path}");
            var watch = new Stopwatch();
            watch.Start();

            
                
            for(int attemptNum = 0; attemptNum<MaxAttemptsCount; attemptNum++)
            {
                    

                if(attemptNum != 0) LogString($"Повторяю запрос {id}");

                HttpResponseMessage response;

                using (var client = new HttpClient())
                {
                    if (ShouldAuthorize && _token != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.access_token);
                    client.Timeout = TimeSpan.FromMilliseconds(_httpTimeout);

                    try
                    {
                        var content = new StringContent(serializedJSON);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response = client.PostAsync(_apiUrl + path, content).Result;
                        
                    }catch(Exception exp)
                    {
                        LogException(exp);
                        Thread.Sleep(100);
                        continue;
                    }
                }

                //Обработка ответа от сервера
                if (response.IsSuccessStatusCode)
                {
                    LogString($"Запрос {id} выполнен успешно за {watch.ElapsedMilliseconds} мс");
                    return response;
                }
                if ((int)response.StatusCode == 401)
                {
                    LogString($"Запрос {id} не выполнен (статус: 401) за {watch.ElapsedMilliseconds} мс", prefix: "ERROR");
                    throw new UnauthorizedException(_username);
                }
                if ((int)response.StatusCode == 403)
                {
                    LogString($"Запрос {id} не выполнен (статус: 403) за {watch.ElapsedMilliseconds} мс", prefix: "ERROR");
                    throw new ForbbidenException(_username);
                }
                LogHttpError(response);
            }
            


            LogString($"Запрос {id} не выполнен. Превышено максимальное количество попыток. Времени прошло: {watch.ElapsedMilliseconds} мс", prefix: "ERROR");
            throw new MaxAttemptsException(_username);
        }

        private string typeToString(LogType type)
        {
            switch (type)
            {
                case LogType.DEBUG:
                    return "DEBUG";
                case LogType.INFO:
                    return "INFO";
                case LogType.WARN:
                    return "WARN";
                case LogType.ERROR:
                    return "ERROR";
                case LogType.FATAL:
                    return "FATAL";
            }
            throw new InvalidEnumArgumentException();
        }

        private void LogHttpError(HttpResponseMessage result) => LogString("Made request but answer was: " + result.StatusCode + " " + result.Content.ReadAsStringAsync().Result, prefix: "ERROR");
        private void LogException(Exception exp)
        {
            LogString(exp.Message, prefix: "ERROR");
            if (exp.InnerException != null)
                LogException(exp.InnerException);
        }

        private void LogString(string message, string prefix = "INFO") => LogCallback?.Invoke($"{prefix} [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {_username}: {message}");
    }

}
