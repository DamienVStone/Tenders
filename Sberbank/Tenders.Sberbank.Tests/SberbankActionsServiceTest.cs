using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Models;
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
        private ISberbankActionsService actionsService;
        private CancellationTokenSource cts;
        private CancellationToken ct;

        public SberbankActionsServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<ISberbankActionsService, SberbankActionsService>();
                sc.AddSingleton<ISberbankHttpClientService, SberbankHttpClientService>();
                sc.AddSingleton<ISberbankConfigService, SberbankConfigService>();
                sc.AddSingleton<ISberbankXmlService, SberbankXmlService>();
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
                sc.AddSingleton<IProxyService, ProxyService>();
            });

            actionsService = Container.GetService<ISberbankActionsService>();
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        [Fact]
        public void IsAuthenticationSuccessTest()
        {
            _authWithChecks(actionsService, ct);
        }

        [Fact]
        public async void IsAuctionSearchSuccessTest()
        {
            _authWithChecks(actionsService, ct);
            var result = await actionsService.SearchAsync(new SearchParameters()
            {
                NotificationNumber = "0332100009818000063"
            }, ct);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result.Count() == 1);
        }

        [Fact]
        public async void IsTradePlaceDataRequestSuccessTest()
        {
            _authWithChecks(actionsService, ct);
            // TODO: ������������� ������� �����!!!
            var auctions = await actionsService.SearchAsync(new SearchParameters()
            {
                NotificationNumber = _auctionNumber
            }, ct);
            Assert.NotNull(auctions);
            Assert.NotEmpty(auctions);
            Assert.True(auctions.Count() == 1);
            Assert.False(cts.IsCancellationRequested);

            var auction = auctions.First();
            var tradeData = await actionsService.GetTradeDataAsync(auction, ct);
        }

        [Fact]
        public async void IsBidSuccessTest()
        {
            _authWithChecks(actionsService, ct);

            var auctions = await actionsService.SearchAsync(new SearchParameters()
            {
                NotificationNumber = _auctionNumber
            }, ct);
            Assert.NotNull(auctions);
            Assert.NotEmpty(auctions);
            Assert.True(auctions.Count() == 1);
            Assert.False(cts.IsCancellationRequested);

            var auction = auctions.First();

            ITradePlace tradeData;
            try
            {
                tradeData = await actionsService.GetTradeDataAsync(auction, ct);
            }
            catch (Exception e)
            {
                if (!e.ToString().ToLowerInvariant().Contains("����� ��������"))
                    throw;

                return;
            }

            var bidResult = await actionsService.BidAsync(_auctionBid, auction.ASID, tradeData, ct);
        }

        [Fact]
        public void CanUploadAndSignDocument()
        {
            _authWithChecks(actionsService, ct);
            var file = actionsService.UploadFileAsync(@"C:\ASDocs\Add.docs.zip", "ctl00$phDataZone$Upload", ct).Result;
            Assert.NotNull(file);
            Assert.True(file.IsUploaded);
            Assert.False(string.IsNullOrEmpty(file.UploadedSign));
            Assert.False(string.IsNullOrEmpty(file.UploadedSignFingerprint));
        }

        [Fact]
        public void CanRequest()
        {
            _authWithChecks(actionsService, ct);
            actionsService.MakeRequest(
                new Lot()
                {
                    Url = new Uri("http://www.sberbank-ast.ru/tradezone/supplier/PurchaseRequestEF.aspx?purchID=6403806"),
                    NotificationNumber = "0171100000719000001",
                    Text = "�������� ����� �� ������������� ����������� ����������� ��������������� ���������� ������������ ������� (�����)"
                },
                ct
            ).Wait();
        }

        [Fact]
        public void CanGuestSearch()
        {
            var from = DateTime.Now.Date;
            var to = DateTime.Now.AddDays(1).Date;
            var searchParams = new SearchParameters()
            {
                Text = "����������� ����� �����",
                PublicDateFrom = from,
                PublicDateTo = to
            };

            var result = actionsService.GuestSearchAsync(searchParams, ct).Result;
        }

        private static void _authWithChecks(ISberbankActionsService actionsService, CancellationToken ct)
        {
            var t = actionsService.AuthenticateAsync(ct);
            try
            {
                t.Wait();
                if (t.IsFaulted)
                    Debug.Print(t.Exception.ToString());
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                throw;
            }


            Assert.False(t.IsFaulted);
            Assert.False(ct.IsCancellationRequested);
        }
    }
}
