using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Helpers;
using Tenders.Integration.API.Interfaces;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;

namespace Tenders.Sberbank.Services
{
    public class SberbankActionsService : ISberbankActionsService
    {
        private readonly ISberbankHttpClientService httpClientService;
        private readonly ISberbankDataProvider dataProvider;
        private readonly ISberbankConfigService configService;
        private readonly ISberbankDeserializationService serializationService;
        private readonly IAPIDataProviderService apiDataProvider;
        private readonly ILoggerService logger;

        public SberbankActionsService(
            ISberbankHttpClientService httpClientService,
            ISberbankDataProvider dataProvider,
            ISberbankConfigService configService,
            ISberbankDeserializationService serializationService,
            IAPIDataProviderService apiDataProvider,
            ILoggerService logger
            )
        {
            this.httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            this.dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            this.apiDataProvider = apiDataProvider ?? throw new ArgumentNullException(nameof(apiDataProvider));
            this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AuthenticateAsync(CancellationToken ct)
        {
            await logger.Log("Авторизация на площадке");
            ct.ThrowIfCancellationRequested();
            await apiDataProvider.Authenticate(ct);
            var step1GetResult = await httpClientService.GetAsync(configService.AuthStep1Url, ct);
            _throwIfDocumentError(step1GetResult);
            var step1PostResult = await httpClientService.PostAsync(configService.AuthStep1Url, await _getAuthStep2Form(step1GetResult, ct), ct);
            var step2GetResult = await httpClientService.GetAsync(configService.AuthStep2Url, ct);
            _throwIfDocumentError(step1GetResult);
            var step3PostResult = await httpClientService.PostAsync(configService.AuthStep3Url, _getAuthStep3Form(step2GetResult), ct);
            _throwIfDocumentError(step3PostResult);

            ct.ThrowIfCancellationRequested();
            await logger.Log("Авторизован как: " + step3PostResult.GetTextById("ctl00_loginctrl_link"));
        }

        public Task<ISearchResult> SearchAsync(ISearchParameters parameters, CancellationToken ct)
        {
            try
            {
                logger.Log("Поиск аукциона " + parameters.Regnumber).Wait();
                ct.ThrowIfCancellationRequested();
                var step1GetResult = httpClientService.GetAsync(configService.PurchaseRequestListUrl, ct).Result;
                _throwIfDocumentError(step1GetResult);
                var step2PostResult = httpClientService.PostAsync(configService.PurchaseRequestListUrl, _getSearchForm(step1GetResult, parameters), ct).Result;
                _throwIfDocumentError(step2PostResult);
                var xmlFilterResult = HttpUtility.HtmlDecode(step2PostResult.GetTextById("ctl00_ctl00_phWorkZone_xmlData"));
                var result = serializationService.GetSearchResult(xmlFilterResult);
                logger.Log("Найдено аукционов: " + (result?.Entries?.Length ?? 0)).Wait();
                return Task.Run(() => result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ITradePlace> GetTradeDataAsync(ISearchResultEntry auction, CancellationToken ct)
        {
            await logger.Log("Получение данных о торгах");
            var templ = configService.GetAsyncRefreshData(auction.reqID, _getTime().ToString());
            var jsonContent = new StringContent(templ, Encoding.UTF8, "application/json");
            var postUrl = configService.GetTradePlaceUrl(auction.reqID, auction.ASID);
            var postResult = await httpClientService.PostAsync(postUrl, jsonContent, ct);
            var result = serializationService.GetTradePlace(postResult.Text);
            await logger.Log("Данные о торгах получены");
            return result;
        }

        public async Task<ITradePlace> BidAsync(decimal price, string ASID, ITradePlace tradeData, CancellationToken ct)
        {
            await logger.Log($"Подача предложения {price} на аукционе {tradeData.PurchCode}");
            if (string.IsNullOrEmpty(ASID))
                throw new ArgumentException(nameof(ASID));

            if (tradeData == null)
                throw new ArgumentNullException(nameof(tradeData));

            var data = configService.GetBidData(price, tradeData);
            var hash = await apiDataProvider.SignAsync(data, ct);

            var bid = new Bid
            {
                reqID = tradeData.ReqID,
                xmlData = data,
                Hash = hash
            };

            var jsonData = JsonConvert.SerializeObject(bid);
            var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var postUrl = configService.GetTradePlaceBidUrl(tradeData.ReqID, ASID);
            var postResult = await httpClientService.PostAsync(postUrl, jsonContent, ct);
            var result = serializationService.GetTradePlace(postResult.Text);
            return result;
        }

        // TODO: наверное стоит такие штуки перенести в DataProviderService
        private FormUrlEncodedContent _getSearchForm(HtmlDocument doc, ISearchParameters parameters)
        {
            var form = new Dictionary<string, string>()
            {
                { "_EVENTTARGET" , doc.GetValueById("_EVENTTARGET", true) },
                { "__EVENTARGUMENT" , doc.GetValueById("__EVENTARGUMENT", true) },
                { "__VIEWSTATEFIELDCOUNT" , doc.GetValueById("__VIEWSTATEFIELDCOUNT", true) },
                { "__VIEWSTATE" , doc.GetValueById("__VIEWSTATE", true) },
                { "__VIEWSTATEGENERATOR" , doc.GetValueById("__VIEWSTATEGENERATOR", true) },
                { "ctl00$ctl00$phWorkZone$xmlFilter" , configService.GetSearchXml(parameters) },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$tbxPurchaseCode" , parameters.Regnumber },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$tbSearch" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbSearch")},
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$purchamountstart" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountstart") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$purchamountend" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountend") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$cldPublicDateStart" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateStart") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$cldPublicDateEnd" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateEnd") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldRRequestDateStart" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRRequestDateStart") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldRequestDateEnd" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRequestDateEnd") },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldAuctionBeginDateStart" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateStart")},
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldAuctionBeginDateEnd" , doc.GetValueById("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateEnd") },
                { "ctl00$ctl00$phWorkZone$btnSearch" , doc.GetValueById("ctl00_ctl00_phWorkZone_btnSearch") },
                { "ctl00$ctl00$phWorkZone$hfFilterZone" , doc.GetValueById("ctl00_ctl00_phWorkZone_hfFilterZone") },
                { "ctl00$ctl00$phWorkZone$xmlData" , doc.GetValueById("ctl00_ctl00_phWorkZone_xmlData") }
            };

            if (int.TryParse(form["__VIEWSTATEFIELDCOUNT"], out int viewstateFieldCount))
                for (int i = 1; i < viewstateFieldCount; i++)
                    form[$"__VIEWSTATE{i}"] = doc.GetValueById($"__VIEWSTATE{i}", true);

            return new FormUrlEncodedContent(form);
        }

        private void _throwIfDocumentError(HtmlDocument doc)
        {
            var error = doc.GetTextById("ctl00_phWorkZone_lblErrorMsg", true)?.Trim();
            if (string.IsNullOrEmpty(error))
                return;

            throw new Exception(error);
        }

        private void _throwIfDocumentError(string error)
        {
            throw new Exception(error);
        }

        private async Task<FormUrlEncodedContent> _getAuthStep2Form(HtmlDocument doc, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var form1Action = doc.GetElementbyId("form1").GetAttributeValue("action", string.Empty).Replace("./", "https://login.sberbank-ast.ru/");
            var mainContent_xmlData = configService.GetLogonRegisterData(doc.GetTextById("hiddenNow"), doc.GetTextById("hiddenTicket"));
            return new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "__VIEWSTATE", doc.GetValueById("__VIEWSTATE") },
                { "__VIEWSTATEGENERATOR", doc.GetValueById("__VIEWSTATEGENERATOR") },
                { "ctl00$mainContent$RadioButtonList2", doc.GetValueById("mainContent_RadioButtonList2_0") },
                { "ctl00$mainContent$txtLoginName", doc.GetValueById("mainContent_txtLoginName") },
                { "ctl00$mainContent$txtPassword", doc.GetValueById("mainContent_txtPassword") },
                { "ctl00$mainContent$DDL1", await apiDataProvider.GetFingerprintAsync(ct) },
                { "ctl00$mainContent$xmlData", mainContent_xmlData },
                { "ctl00$mainContent$signValue", await apiDataProvider.SignAsync(mainContent_xmlData, ct) }
            });
        }

        private FormUrlEncodedContent _getAuthStep3Form(HtmlDocument doc)
        {
            return new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                            { "wa", doc.GetValueByXPath("//*[@name='wa']") },
                            { "wresult", doc.GetValueByXPath("//*[@name='wresult']") },
                            { "StsCertsChecked", doc.GetValueByXPath("//*[@name='StsCertsChecked']") },
                    });
        }

        private static Int64 _getTime()
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }
    }
}
