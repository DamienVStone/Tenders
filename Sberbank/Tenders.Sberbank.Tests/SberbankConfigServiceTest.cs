using Microsoft.Extensions.DependencyInjection;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;
using Tenders.Sberbank.Services;
using Xunit;

namespace Tenders.Sberbank.Tests
{
    public class SberbankConfigServiceTest
    {
        public SberbankConfigServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IConfigService, ConfigService>();
                sc.AddSingleton<ISberbankConfigService, SberbankConfigService>();
            });
        }

        [Fact]
        public void IsConfigsFilled()
        {
            var service = Container.GetService<ISberbankConfigService>();
            Assert.NotNull(service.AuctionInfo);

            Assert.False(string.IsNullOrEmpty(service.AuthStep1Url));
            Assert.False(string.IsNullOrEmpty(service.AuthStep2Url));
            Assert.False(string.IsNullOrEmpty(service.AuthStep3Url));
            Assert.False(string.IsNullOrEmpty(service.PurchaseRequestListUrl));
            Assert.False(string.IsNullOrEmpty(service.GetLogonRegisterData("time", "ticket")));

            Assert.False(string.IsNullOrEmpty(service.GetTradePlaceUrl("TRADE_ID", "ASID")));
            Assert.False(string.IsNullOrEmpty(service.GetTradePlaceBidUrl("TRADE_ID", "ASID")));
            //Assert.False(string.IsNullOrEmpty(service.GetBidData("time", "ticket")));
            Assert.False(string.IsNullOrEmpty(service.GetAsyncRefreshData("TRADE_ID", "MILLISECONDS")));

            Assert.False(string.IsNullOrEmpty(service.GetSearchXml(new SearchParameters()
            {
                Regnumber = "000000000000"
            })));
        }
    }
}
