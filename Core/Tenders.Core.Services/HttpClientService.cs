using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Tenders.Core.Abstractions.Services;

namespace Tenders.Core.Services
{
    public abstract class HttpClientService : IHttpClientService
    {
        private readonly Lazy<HttpClient> _client;
        private HttpClient client => _client.Value;
        protected virtual bool UseProxy => true;

        public HttpClientService()
        {
            _client = new Lazy<HttpClient>(() => _createClient().Result, true);
        }

        public Task<HtmlDocument> GetAsync(Uri url, CancellationToken ct)
        {
            return GetAsync(url.ToString(), ct);
        }

        public virtual async Task<HtmlDocument> GetAsync(string url, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{((int)response.StatusCode)} {response.StatusCode.ToString()} - {response.ReasonPhrase}");

            var result = new HtmlDocument();
            result.Load(await response.Content.ReadAsStreamAsync());
            return result;
        }

        public Task<HtmlDocument> PostAsync(Uri url, HttpContent content, CancellationToken ct)
        {
            return PostAsync(url.ToString(), content, ct);
        }

        public virtual async Task<HtmlDocument> PostAsync(string url, HttpContent data, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var response = await client.PostAsync(url, data, ct);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{((int)response.StatusCode)} {response.StatusCode.ToString()} - {response.ReasonPhrase}");

            var result = new HtmlDocument();
            result.Load(await response.Content.ReadAsStreamAsync());
            return result;
        }

        protected virtual HttpClientHandler GetHttpClientHandler(WebProxy proxy)
        {
            var result = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true
            };

            if (proxy != null)
                result.Proxy = proxy;

            return result;
        }

        protected abstract Task<WebProxy> GetWebProxy(CancellationToken ct);

        protected virtual void SetDefaultHeaders(HttpClient client)
        {
        }

        public virtual void AddDefaultHeaders(string key, string value)
        {
            client.DefaultRequestHeaders.Remove(key);
            client.DefaultRequestHeaders.Add(key, value);
        }

        private async Task<HttpClient> _createClient()
        {
            var cts = new CancellationTokenSource();
            var proxy = await GetWebProxy(cts.Token);
            var handler = GetHttpClientHandler(proxy);
            var result = new HttpClient(handler);
            SetDefaultHeaders(result);
            return result;
        }
    }
}
