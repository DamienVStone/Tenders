using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Tenders.Core.DI;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;
using Xunit;

namespace Tenders.Integration.API.Tests
{
    public class ProxyServiceTest
    {
        public ProxyServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<IProxyService, ProxyService>();
            });
        }
        [Fact]
        public void GetProxyTest()
        {
            var proxyService = Container.GetService<IProxyService>();
            Assert.NotNull(proxyService);
            var cts = new CancellationTokenSource();
            var proxy = proxyService.GetProxy(cts.Token).Result;
            Assert.NotNull(proxy);
        }
    }
}
