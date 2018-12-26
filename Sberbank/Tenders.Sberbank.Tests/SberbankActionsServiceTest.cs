using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;
using Tenders.Sberbank.Services;
using Xunit;

namespace Tenders.Sberbank.Tests
{
    public class SberbankActionsServiceTest
    {

        private string _auctionNumber = "0166400000118000046";
        private decimal _auctionBid = 648582.77m;

        public SberbankActionsServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<ISberbankActionsService, SberbankActionsService>();
                sc.AddSingleton<ISberbankHttpClientService, SberbankHttpClientService>();
                sc.AddSingleton<ISberbankDataProvider, SberbankDataProvider>();
                sc.AddSingleton<ISberbankConfigService, SberbankConfigService>();
                sc.AddSingleton<ISberbankDeserializationService, SberbankDeserializationService>();
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
                sc.AddSingleton<IProxyService, ProxyService>();
            });
        }

        [Fact]
        public void IsAuthenticationSuccessTest()
        {
            var actionsService = Container.GetService<ISberbankActionsService>();
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            actionsService.AuthenticateAsync(ct).Wait();
            Assert.False(cts.IsCancellationRequested);
        }

        [Fact]
        public async void IsAuctionSearchSuccessTest()
        {
            var actionsService = Container.GetService<ISberbankActionsService>();
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            actionsService.AuthenticateAsync(ct).Wait();
            Assert.False(cts.IsCancellationRequested);
            var result = await actionsService.SearchAsync(new SearchParameters()
            {
                Regnumber = "0332100009818000063"
            }, ct);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Entries);
            Assert.True(result.Entries.Length == 1);
        }

        [Fact]
        public async void IsTradePlaceDataRequestSuccessTest()
        {
            var actionsService = Container.GetService<ISberbankActionsService>();
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            actionsService.AuthenticateAsync(ct).Wait();
            Assert.False(cts.IsCancellationRequested);

            var auctions = await actionsService.SearchAsync(new SearchParameters()
            {
                Regnumber = _auctionNumber
            }, ct);
            Assert.NotNull(auctions);
            Assert.NotEmpty(auctions.Entries);
            Assert.True(auctions.Entries.Length == 1);
            Assert.False(cts.IsCancellationRequested);

            var auction = auctions.Entries[0];
            var tradeData = await actionsService.GetTradeDataAsync(auction, ct);
        }

        [Fact]
        public async void IsBidSuccessTest()
        {
            var actionsService = Container.GetService<ISberbankActionsService>();
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            actionsService.AuthenticateAsync(ct).Wait();
            Assert.False(cts.IsCancellationRequested);

            var auctions = await actionsService.SearchAsync(new SearchParameters()
            {
                Regnumber = _auctionNumber
            }, ct);
            Assert.NotNull(auctions);
            Assert.NotEmpty(auctions.Entries);
            Assert.True(auctions.Entries.Length == 1);
            Assert.False(cts.IsCancellationRequested);

            var auction = auctions.Entries[0];

            ITradePlace tradeData;
            try
            {
                tradeData = await actionsService.GetTradeDataAsync(auction, ct);
            }
            catch (Exception e)
            {
                if (!e.ToString().ToLowerInvariant().Contains("торги начнутся"))
                    throw;

                return;
            }

            var bidResult = await actionsService.BidAsync(_auctionBid, auction.ASID, tradeData, ct);
        }
    }
}
