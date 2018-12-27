using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Tenders.Core.DI;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;
using Xunit;

namespace Tenders.Integration.API.Tests
{
    public class DataProviderTest
    {
        public DataProviderTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddTransient<IProxyService, ProxyService>();
                sc.AddTransient<IAPIDataProviderService, APIDataProviderService>();
            });
        }

        [Fact]
        public void Authenticate()
        {
            var service = Container.GetService<IAPIDataProviderService>();
            Assert.NotNull(service);

            var cts = new CancellationTokenSource();
            service.Authenticate(cts.Token).Wait();
            Assert.False(cts.IsCancellationRequested);
        }

        [Fact]
        public void GetFingerprintAsyncTest()
        {
            var service = Container.GetService<IAPIDataProviderService>();
            Assert.NotNull(service);

            var cts = new CancellationTokenSource();
            service.Authenticate(cts.Token).Wait();
            Assert.False(cts.IsCancellationRequested);
            var task = service.GetFingerprintAsync(cts.Token);
            Assert.NotNull(task);
            var fingerprint = task.Result;
            Assert.False(cts.IsCancellationRequested);
            Assert.False(string.IsNullOrEmpty(fingerprint));
        }

        [Fact]
        public void SignTextAsyncTest()
        {
            var service = Container.GetService<IAPIDataProviderService>();
            Assert.NotNull(service);

            var cts = new CancellationTokenSource();
            service.Authenticate(cts.Token).Wait();
            Assert.False(cts.IsCancellationRequested);
            var task = service.SignAsync("TextForSign", cts.Token);
            Assert.NotNull(task);
            var result = task.Result;
            Assert.False(cts.IsCancellationRequested);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void SyncronizeByKeyAsyncTest()
        {
            var service = Container.GetService<IAPIDataProviderService>();
            Assert.NotNull(service);

            var cts = new CancellationTokenSource();
            service.Authenticate(cts.Token).Wait();
            Assert.False(cts.IsCancellationRequested);
            var task = service.SyncronizeByKeyAsync("key", cts.Token);
            Assert.NotNull(task);
            task.Wait();
            Assert.False(cts.IsCancellationRequested);
        }
    }
}
