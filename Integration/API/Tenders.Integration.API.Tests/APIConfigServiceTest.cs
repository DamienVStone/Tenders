using Microsoft.Extensions.DependencyInjection;
using System;
using Tenders.Core.DI;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;
using Xunit;

namespace Tenders.Integration.API.Tests
{
    public class APIConfigServiceTest
    {
        public APIConfigServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
            });
        }
        [Fact]
        public void ConfigsNotEmptyOrNull()
        {
            var service = Container.GetService<IAPIConfigService>();
            Assert.NotNull(service);
            Assert.IsType<Uri>(service.GetFingerprint);
            Assert.NotNull(service.GetFingerprint);
            Assert.IsType<Uri>(service.GetProxy);
            Assert.NotNull(service.GetProxy);
            Assert.IsType<Uri>(service.Host);
            Assert.NotNull(service.Host);
            Assert.IsType<Uri>(service.SignText);
            Assert.NotNull(service.SignText);
            Assert.IsType<Uri>(service.SynchronizeByKey);
            Assert.NotNull(service.SynchronizeByKey);
            Assert.IsType<Uri>(service.Token);
            Assert.NotNull(service.Token);

            Assert.True(!string.IsNullOrEmpty(service.Password));
            Assert.True(!string.IsNullOrEmpty(service.SecurityToken));
            Assert.True(!string.IsNullOrEmpty(service.Username));
        }
    }
}
