using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Integration.API.Interfaces;

namespace Tenders.Integration.API.Services
{
    public class ProxyService : IProxyService
    {
        private readonly IAPIConfigService configService;

        public ProxyService(
            IAPIConfigService configService
            )
        {
            this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        public async Task<WebProxy> GetProxy(CancellationToken ct)
        {
            var data = await new HttpClient().GetStringAsync($"{configService.GetProxy}?token={configService.SecurityToken}");
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
        }
    }
}
