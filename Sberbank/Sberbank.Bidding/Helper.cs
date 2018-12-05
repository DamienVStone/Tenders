using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sberbank.Bidding
{
    public static class Helper
    {//test
        public static class Constants
        {
            #region sber
            public const string SBER_AUTH_STEP1_URL = "https://login.sberbank-ast.ru/Login.aspx?ReturnUrl=%2f%3fwa%3dwsignin1.0%26wtrealm%3dhttp%253a%252f%252fwww.sberbank-ast.ru%252f%26wreply%3daHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA%253d%253d&wa=wsignin1.0&wtrealm=http%3a%2f%2fwww.sberbank-ast.ru%2f&wreply=aHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA%3d%3d";
            public const string SBER_AUTH_STEP2_URL = "https://login.sberbank-ast.ru/default.aspx/?wa=wsignin1.0&wtrealm=http://www.sberbank-ast.ru/&wreply=aHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA==";
            public const string SBER_AUTH_STEP3_URL = "http://www.sberbank-ast.ru/tradezone/default.aspx";
            public const string SBER_LOGON_REGISTER_TEXT = "<logonregister><request>Просьба обеспечить вход в личный кабинет Электронной торговой площадки «Сбербанк-АСТ». Подлинность и достоверность запроса на аутентификацию подтверждаю.</request><now>{{NOW}}</now><ticket>{{TICKET}}</ticket></logonregister>";
            public const string SBER_COOKIE_TEMPLATE = "_ym_uid=1502459726551214073; _ga=GA1.2.238098008.1502459722; __utma=99173852.238098008.1502459722.1523281306.1527755093.125; _ym_d=1530176747; ASP.NET_SessionId=f4n0j0sn54m5mrvp1jzfv3jk; {{AUTH_EXTRA_INFO}}";
            #endregion

            #region proxy
            public const string PROXY_URL = "";
            public const string PROXY_LOGIN = "";
            public const string PROXY_PASSWORD = "";
            #endregion

            #region API
            //public const string API_URL = "http://localhost:50183/";
            public static readonly string API_URL = "";
            public static readonly string API_SIGN_TEXT_URL = API_URL + "";
            public static readonly string API_GET_FINGERPRINT_URL = API_URL + "";
            public static readonly string API_TOKEN_URL = API_URL + "Token";
            //public static readonly string _login = Environment.GetEnvironmentVariable("env_api_login");
            //public const string _password = Environment.GetEnvironmentVariable("env_api_password");
            public static readonly string API_LOGIN = "";
            public static readonly string API_PASSWORD = "";
            #endregion
        }

        public static class Http
        {
            public static WebProxy Proxy = new WebProxy()
            {
                Address = new Uri(Constants.PROXY_URL),
                BypassProxyOnLocal = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Constants.PROXY_LOGIN, Constants.PROXY_PASSWORD)
            };

            public static HttpClientHandler Handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                Proxy = Proxy
            };

            private static object _sberbankClientLocker = new object();
            private static HttpClient _sberbankClient;
            public static HttpClient GetSberbankClient()
            {
                lock (_sberbankClientLocker)
                {
                    if (_sberbankClient == null)
                    {
                        _sberbankClient = new HttpClient(Handler);
                        _sberbankClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                        _sberbankClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                        _sberbankClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                        _sberbankClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                        _sberbankClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                        _sberbankClient.DefaultRequestHeaders.Add("Host", "login.sberbank-ast.ru");
                        _sberbankClient.DefaultRequestHeaders.Add("Referer", "http://www.sberbank-ast.ru/Default.aspx?");
                        _sberbankClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                        _sberbankClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36");
                    }
                }

                return _sberbankClient;
            }

            private static object _apiClientLocker = new object();
            private static HttpClient _apiClient;
            public static HttpClient GetApiClient()
            {
                lock (_apiClientLocker)
                {
                    if (_apiClient == null)
                    {
                        _apiClient = new HttpClient(Handler);
                        _apiClient.DefaultRequestHeaders.Add("Accept", "text/plain");
                    }
                }

                return _apiClient;
            }

            public static async Task<Stream> RequestGet(Uri url, HttpClient client, CancellationToken ct)
            {
                return await Logger.LogElapsed(async () =>
                {
                    var response = await client.GetAsync(url, ct);
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new HttpRequestException(response.ReasonPhrase);

                    return await response.Content.ReadAsStreamAsync();
                }, $"GET {url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}");
            }

            public static async Task<Stream> RequestPost(Uri url, HttpContent data, HttpClient client, CancellationToken ct)
            {
                return await Logger.LogElapsed(async () =>
                {
                    var response = await client.PostAsync(url, data, ct);

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new HttpRequestException(response.ReasonPhrase);

                    return await response.Content.ReadAsStreamAsync();
                }, $"POST {url.Scheme}://{url.Host}:{url.Port}{url.AbsolutePath}");
            }
        }

        public static class Logger
        {
            private static ConcurrentBag<string> _logs = new ConcurrentBag<string>();
            public static async void Log(string message)
            {
                await Task.Run(() =>
                {
                    _logs.Add(message);
                    Console.WriteLine(message);
                });
            }

            public static Task<T> LogElapsed<T>(Func<Task<T>> a, string message)
            {
                Log($"Message={message};State=\"Started\"");
                var sw = new Stopwatch();
                sw.Start();
                var task = a();
                task.ContinueWith(t =>
                {
                    sw.Stop();
                    Log($"Message={message};State=\"Completed\";Elapsed={sw.Elapsed}");
                });

                return task;
            }
        }

        public static class Api
        {
            private static Token _token = default(Token);
            private static HttpClient _client;

            static Api()
            {
                _client = Http.GetApiClient();
            }

            public static async Task<string> SignAsync(string data, CancellationToken ct)
            {
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Data", data) });
                using (var response = await Http.RequestPost(new Uri(Constants.API_SIGN_TEXT_URL), content, _client, ct))
                    return JsonConvert
                        .DeserializeObject<StringResponse>(await new StreamReader(response, Encoding.UTF8).ReadToEndAsync())
                        .Data;
            }

            public static async Task<string> GetFingerprintAsync(CancellationToken ct)
            {
                using (var s = await Http.RequestGet(new Uri(Constants.API_GET_FINGERPRINT_URL), _client, ct))
                    return JsonConvert
                        .DeserializeObject<StringResponse>(await new StreamReader(s, Encoding.UTF8).ReadToEndAsync())
                        .Data;
            }

            public static async Task AuthenticateAsync(CancellationToken ct)
            {
                using (var response = await Http.RequestPost(new Uri(Constants.API_TOKEN_URL), TokenRequest.AsContent(), _client, ct))
                {
                    await new StreamReader(response, Encoding.UTF8).ReadToEndAsync()
                        .ContinueWith(t =>
                        {
                            _token = JsonConvert.DeserializeObject<Token>(t.Result);
                            _addAuth(_client);
                        });
                }
            }

            private static void _addAuth(HttpClient c)
            {
                if (_token == null)
                    throw new UnauthorizedAccessException("You should call Authenticate(...) before!");

                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_token.token_type, _token.access_token);
            }

            private class Token
            {
                public string access_token { get; set; }
                public string token_type { get; set; }
                public int expires_in { get; set; }
                public string userName { get; set; }
                public string issued { get; set; }
                public string expires { get; set; }
            }
            private class TokenRequest
            {
                public string grant_type;
                public string username;
                public string password;

                public static TokenRequest Create()
                {
                    return new TokenRequest()
                    {
                        grant_type = "password",
                        password = Constants.API_PASSWORD,
                        username = Constants.API_LOGIN
                    };
                }

                public static FormUrlEncodedContent AsContent()
                {
                    return new FormUrlEncodedContent(new[] {
                         new KeyValuePair<string, string>("grant_type", "password"),
                         new KeyValuePair<string, string>("password", Constants.API_PASSWORD),
                         new KeyValuePair<string, string>("username", Constants.API_LOGIN)
                    });
                }
            }

            private class StringResponse
            {
                public string Data { get; set; }
            }
        }
    }
}
