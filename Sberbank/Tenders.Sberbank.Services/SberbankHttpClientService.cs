using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Sberbank.Abstractions.Services;

namespace Tenders.Sberbank.Services
{
    public class SberbankHttpClientService : HttpClientService, ISberbankHttpClientService
    {
        private readonly ILoggerService logger;
        private readonly IProxyService proxyService;
        private readonly ISberbankConfigService sberbankConfigService;

        public SberbankHttpClientService(
            ILoggerService logger,
            IProxyService proxyService,
            ISberbankConfigService sberbankConfigService
            )
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.proxyService = proxyService ?? throw new System.ArgumentNullException(nameof(proxyService));
            this.sberbankConfigService = sberbankConfigService ?? throw new System.ArgumentNullException(nameof(sberbankConfigService));
        }

        protected override async Task<WebProxy> GetWebProxy(CancellationToken ct)
        {
            if (sberbankConfigService.IsDebug)
                return null;

            return await proxyService.GetProxy(ct);
        }

        protected override void SetDefaultHeaders(HttpClient client)
        {
            base.SetDefaultHeaders(client);

            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");//, deflate");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Referer", "http://www.sberbank-ast.ru/Default.aspx?");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110 Safari/537.36");
        }
    }
}
