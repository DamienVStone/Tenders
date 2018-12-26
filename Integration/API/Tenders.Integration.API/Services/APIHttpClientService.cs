using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;

namespace Tenders.Integration.API.Services
{
    public class APIHttpClientService : HttpClientService, IAPIHttpClientService
    {
        private readonly IProxyService proxyService;
        private readonly IAPIConfigService configService;

        public APIHttpClientService(
            IProxyService proxyService,
            IAPIConfigService configService
        )
        {
            this.proxyService = proxyService ?? throw new System.ArgumentNullException(nameof(proxyService));
            this.configService = configService ?? throw new System.ArgumentNullException(nameof(configService));
        }

        protected override async Task<WebProxy> GetWebProxy(CancellationToken ct)
        {
            if (configService.UseProxy)
                return await proxyService.GetProxy(ct);
            //else if (!string.IsNullOrEmpty(configService.DebugProxyHost))
            //    return new WebProxy(configService.DebugProxyHost)
            //    {
            //        Credentials = new NetworkCredential(configService.DebugProxyUsername, configService.DebugProxyPassword)
            //    };

            return null;
        }

        protected override void SetDefaultHeaders(HttpClient client)
        {
            base.SetDefaultHeaders(client);
            client.DefaultRequestHeaders.Add("Accept", "text/plain");
        }
    }
}
