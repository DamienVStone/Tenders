using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sberbank.Bidding.Helpers
{
    public static class Http
    {
        public static void Init()
        {
            Proxy = string.IsNullOrEmpty(Constants.CORP_PROXY_URL) ? GetProxy().Result :
             new WebProxy()
             {
                 Address = new Uri(Constants.CORP_PROXY_URL),
                 BypassProxyOnLocal = true,
                 Credentials = new NetworkCredential(Constants.CORP_PROXY_LOGIN, Constants.CORP_PROXY_PASSWORD)
             };

            Handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                Proxy = Proxy
            };
        }
        internal static async Task<WebProxy> GetProxy()
        {
            return await Logger.LogElapsed(async () =>
            {
                Console.WriteLine("try to get proxy...");
                var data = await new HttpClient().GetStringAsync(Constants.API_GET_PROXY_URL + $"{(string.IsNullOrEmpty(Constants.AUCTION_MANAGER_TOKEN) ? "" : "?token=" + Constants.AUCTION_MANAGER_TOKEN)}");
                Console.WriteLine("get proxy " + data);
                var errorText = "Wrong proxy format! Should be - host:port:login@password";
                var p = data.Split(':');
                if (p.Length < 3)
                    throw new FormatException(errorText);

                var host = p[0];
                int.TryParse(p[1], out var port);
                var auth = p[2].Split('@');
                if (auth.Length < 2 && port == 0)
                    throw new FormatException(errorText);

                var login = auth[0];
                var password = auth[1];

                return new WebProxy()
                {
                    Address = new Uri($"http://{host}:{port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(login, password)
                };
            }, "Получение прокси");
        }

        public static void Init(WebProxy p)
        {
            Proxy = p;


        }

        public static WebProxy Proxy;

        public static HttpClientHandler Handler;

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
                    _sberbankClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                    //_sberbankClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                    //_sberbankClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                    _sberbankClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    //_sberbankClient.DefaultRequestHeaders.Add("Host", "login.sberbank-ast.ru");
                    _sberbankClient.DefaultRequestHeaders.Add("Referer", "http://www.sberbank-ast.ru/Default.aspx?");
                    //_sberbankClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
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

        public static async Task<Stream> RequestGet(string url, HttpClient client, CancellationToken ct)
        {
            return await Logger.LogElapsed(async () =>
            {
                var response = await client.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{((int)response.StatusCode)} {response.StatusCode.ToString()} - {response.ReasonPhrase}");

                return await response.Content.ReadAsStreamAsync();
            }, $"GET {url}");
        }

        public static async Task<Stream> RequestPost(string url, HttpContent data, HttpClient client, CancellationToken ct)
        {
            return await Logger.LogElapsed(async () =>
            {
                var response = await client.PostAsync(url, data, ct);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{((int)response.StatusCode)} {response.StatusCode.ToString()} - {response.ReasonPhrase}");

                return await response.Content.ReadAsStreamAsync();
            }, $"POST {url}");
        }

        public static async Task<string> StringRequestGet(string url, HttpClient client, CancellationToken ct)
        {
            return await new StreamReader(await RequestGet(url, client, ct)).ReadToEndAsync();
        }

        public static async Task<string> StringRequestPost(string url, HttpContent data, HttpClient client, CancellationToken ct)
        {
            return await new StreamReader(await RequestPost(url, data, client, ct)).ReadToEndAsync();
        }
    }
}
