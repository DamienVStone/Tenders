using AppLogger.Models;
using System;
using System.Collections.Generic;
using System.Security;

namespace AppLogger.Configuration
{
    class LoggerConfigSection
    {
        private static readonly string _varPrefix = "APPLOGGER";

        private static LoggerConfigSection _section;
        public static LoggerConfigSection Instance
        {
            get {
                if (_section == null) _section = new LoggerConfigSection();
                return _section;
            }
        }
        private LoggerConfigSection() {
            ApiUrl = _getAppLoggerVariableNotNull("API_URL");
            LogLevel = (LogType)Enum.Parse(typeof(LogType), _getAppLoggerVariableNotNull("LOG_LEVEL"));


            var httpTimeout = _getAppLoggerVariable("TIMEOUT_HTTP");
            var logTimeout = _getAppLoggerVariable("TIMEOUT_LOG");
            var statusTimeout = _getAppLoggerVariable("TIMEOUT_STATUS");

            Timeout = new Timeout
            {
                Http = long.Parse(_getAppLoggerVariable("TIMEOUT_HTTP") ?? "3000"),
                Log = long.Parse(_getAppLoggerVariable("TIMEOUT_LOG") ?? "1000"),
                Status = long.Parse(_getAppLoggerVariable("TIMEOUT_STATUS") ?? "1000")
            };

            var configs = new List<LoggerConfig>();
            var allVars = Environment.GetEnvironmentVariables();
            foreach (var key in allVars.Keys)
            {
                if (((string)key).StartsWith($"{_varPrefix}_LOGGER"))
                {
                    var val = (string)allVars[key];
                    try {
                        var parts = val.Split(":");
                        configs.Add(new LoggerConfig { Username = parts[0], Password = parts[1] });
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException("Неверный формат авторизационных данных для логгера. Верный формат: 'USERNAME:PASSWORD'");
                    }
                }
            }
            Loggers = configs;
        }


        public string ApiUrl { get; set; }
        public LogType LogLevel { get; set; }
        public Timeout Timeout { get; set; }
        public List<LoggerConfig> Loggers { get; set; }

        private string _getAppLoggerVariableNotNull(string Name)
        {
            var varName = $"{_varPrefix}_{Name}";
            string val;
            try
            {
                val = Environment.GetEnvironmentVariable(varName);
            }catch(SecurityException exp)
            {
                throw new InvalidOperationException($"Недостаточно прав для получения переменной {varName}. {exp.Message}");
            }
            if (val == null) throw new InvalidOperationException($"Не удалось найти переменную окружения: {varName}");

            return val;
        }

        private string _getAppLoggerVariable(string Name)
        {
            var varName = $"{_varPrefix}_{Name}";
            string val;
            try
            {
                val = Environment.GetEnvironmentVariable(varName);
            }
            catch (SecurityException exp)
            {
                throw new InvalidOperationException($"Недостаточно прав для получения переменной {varName}. {exp.Message}");
            }

            return val;
        }
    }

    class Timeout {
        public long? Log { get; set; }
        public long? Status { get; set; }
        public long? Http { get; set; }
    }

    class LoggerConfig {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
