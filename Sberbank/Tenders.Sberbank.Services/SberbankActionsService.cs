using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Helpers;
using Tenders.Core.Models;
using Tenders.Integration.API.Interfaces;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Models.Requesting;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;
using Tenders.Sberbank.Models.Requesting;

namespace Tenders.Sberbank.Services
{
    public class SberbankActionsService : ISberbankActionsService
    {
        private readonly ISberbankHttpClientService httpClientService;
        private readonly ISberbankConfigService configService;
        private readonly ISberbankXmlService sberbankXmlService;
        private readonly IAPIDataProviderService apiDataProvider;
        private readonly ILoggerService logger;

        public SberbankActionsService(
            ISberbankHttpClientService httpClientService,
            ISberbankConfigService configService,
            ISberbankXmlService sberbankXmlService,
            IAPIDataProviderService apiDataProvider,
            ILoggerService logger
            )
        {
            this.httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            this.apiDataProvider = apiDataProvider ?? throw new ArgumentNullException(nameof(apiDataProvider));
            this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
            this.sberbankXmlService = sberbankXmlService ?? throw new ArgumentNullException(nameof(sberbankXmlService));
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
            _throwIfDocumentError(step1PostResult);
            var step2GetResult = await httpClientService.GetAsync(configService.AuthStep2Url, ct);
            _throwIfDocumentError(step2GetResult);
            var step3PostResult = await httpClientService.PostAsync(configService.AuthStep3Url, _getAuthStep3Form(step2GetResult), ct);
            _throwIfDocumentError(step3PostResult);

            ct.ThrowIfCancellationRequested();
            await logger.Log("Авторизован как: " + step3PostResult.GetTextById("ctl00_loginctrl_link"));
        }

        public async Task<IEnumerable<ISearchResultEntry>> SearchAsync(ISearchParameters parameters, CancellationToken ct)
        {
            await logger.Log("Поиск аукциона " + parameters.NotificationNumber);
            ct.ThrowIfCancellationRequested();
            var step1GetResult = await httpClientService.GetAsync(configService.PurchaseRequestListUrl, ct);
            _throwIfDocumentError(step1GetResult);
            var step2PostResult = await httpClientService.PostAsync(configService.PurchaseRequestListUrl, _getSearchForm(step1GetResult, parameters), ct);
            _throwIfDocumentError(step2PostResult);
            var xmlFilterResult = HttpUtility.HtmlDecode(step2PostResult.GetTextById("ctl00_ctl00_phWorkZone_xmlData"));
            var result = sberbankXmlService.GetSearchResult(xmlFilterResult);
            await logger.Log("Найдено аукционов: " + (result?.Count() ?? 0));
            return result;
        }

        public async Task<IEnumerable<ILot>> GuestSearchAsync(ISearchParameters parameters, CancellationToken ct)
        {
            await logger.Log("Поиск аукциона " + parameters.NotificationNumber);
            ct.ThrowIfCancellationRequested();
            var elasticRequest = new ElasticRequest();
            elasticRequest.Init(parameters);
            var formValues = new Dictionary<string, string>()
            {
                { "xmlData" ,  sberbankXmlService.GetXml(elasticRequest)},
                { "orgId" , "0" },
                { "targetPageCode" , "UnitedPurchaseList" },
            };

            var form = new FormUrlEncodedContent(formValues);
            var postResult = await httpClientService.PostAsync(configService.SearchQueryUrl, form, ct);
            var data = JsonConvert.DeserializeObject<GuestSearchResponse>(postResult.Text);
            var result = data
                .Hits
                .Where(c =>
                {
                    // TODO: реализовать сервис проверки по фильтрам второго порядка
                    var text = c?.fields?.purchName?.ToArray()[0];
                    //return FilterHelper.CheckText(_secondFilters, text);
                    return true;
                })
                .Select(c =>
                {
                    var dt = DateTime.Now.Date.AddHours(1).AddMinutes(1).AddSeconds(1); // чтобы распознать если вдруг не спарсилось
                    var sum = 0m;
                    return new Lot()
                    {
                        ExternalId = c._id,
                        PublishDate = DateTime.TryParse(c?.fields?.PublicDate?.ToArray()[0], out dt) ? dt : DateTime.Now,
                        Sum = decimal.TryParse(c?.fields?.purchAmount?.ToArray()[0].ToString(), out sum) ? sum : 0m,
                        Text = c?.fields?.purchName?.ToArray()[0],
                        Url = new Uri(c.fields?.CreateRequestHrefTerm?.ToArray()[0]),
                        NotificationNumber = c?.fields?.purchCodeTerm?.ToArray()[0],
                        Description = string.Empty,
                        // TODO: Сделать справочник площадок
                        ETP = "ETP_SBAST"
                    };
                });

            return result;
            //await logger.Log("Найдено аукционов: " + (result?.Entries?.Length ?? 0));
        }

        public async Task<ITradePlace> GetTradeDataAsync(ISearchResultEntry auction, CancellationToken ct)
        {
            await logger.Log("Получение данных о торгах");
            var templ = configService.GetAsyncRefreshData(auction.reqID, _getTime().ToString());
            var jsonContent = new StringContent(templ, Encoding.UTF8, "application/json");
            var postUrl = configService.GetTradePlaceUrl(auction.reqID, auction.ASID);
            var postResult = await httpClientService.PostAsync(postUrl, jsonContent, ct);
            var result = sberbankXmlService.GetTradePlace(postResult.Text);
            await logger.Log("Данные о торгах получены: " + result.PurchName);
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
                Hash = hash.Data
            };

            var jsonData = JsonConvert.SerializeObject(bid);
            var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var postUrl = configService.GetTradePlaceBidUrl(tradeData.ReqID, ASID);
            var postResult = await httpClientService.PostAsync(postUrl, jsonContent, ct);
            var result = sberbankXmlService.GetTradePlace(postResult.Text);
            return result;
        }

        public async Task<IAttachableFile> UploadFileAsync(string filePath, string controlName, CancellationToken ct)
        {
            var file = new AttachableFile(filePath, controlName);
            await logger.Log($"Загрузка файла {file.FileName}");
            var getResult = await httpClientService.GetAsync(configService.UploadFileUrl, ct);
            var formData = new MultipartFormDataContent
            {
                { new StringContent(getResult.GetValueById("__EVENTTARGET")), "__EVENTTARGET" },
                { new StringContent(getResult.GetValueById("__EVENTARGUMENT")), "__EVENTARGUMENT" },
                { new StringContent(getResult.GetValueById("__VIEWSTATE")), "__VIEWSTATE" },
                { new StringContent(getResult.GetValueById("__VIEWSTATEGENERATOR")), "__VIEWSTATEGENERATOR" },
                { new StringContent(getResult.GetValueByXPath("//*[@name='ctl00$phDataZone$btnDoUpload']")), "ctl00$phDataZone$btnDoUpload" }
            };

            file.AttachFile(formData);
            var postResult = await httpClientService.PostAsync(configService.UploadFileUrl, formData, ct);
            var targetBlock = postResult.GetElementbyId("ctl00_phDataZone_dataBlock");
            await logger.Log($"Файл {file.FileName} загружен, подписание");
            try
            {
                file.UploadedFileID = targetBlock.GetValueByXPath(".//*[@content='leaf:FileID']", true);
                file.UploadedHash = targetBlock.GetValueByXPath(".//*[@content='leaf:hash']", true);
                file.UploadedHash2012 = targetBlock.GetValueByXPath(".//*[@content='leaf:hash2012']", true);
                var sign = await apiDataProvider.SignAsync(file.UploadedHash2012, ct);
                file.UploadedSign = sign.Data;
                file.UploadedSignFingerprint = sign.Fingerprint;
            }
            catch (Exception)
            {
                throw;
            }

            await logger.Log($"Файл {file.FileName} загружен и подписан");
            return file;
        }

        public async Task MakeRequest(ILot lot, CancellationToken ct)
        {
            await logger.Log($"Подача заявки на {lot.NotificationNumber}");
            var file = await UploadFileAsync(@"C:\ASDocs\Add.docs.zip", "ctl00$phDataZone$Upload", ct);
            var lotDocument = await httpClientService.GetAsync(lot.Url, ct);
            _throwIfDocumentError(lotDocument);
            var xmlData = lotDocument.GetValueById("ctl00_ctl00_phWorkZone_xmlData");
            if (string.IsNullOrEmpty(xmlData))
                throw new ArgumentNullException("xmlData is null!");

            var purchaseRequest = sberbankXmlService.GetPurchaseRequest(xmlData);
            purchaseRequest.Init();
            purchaseRequest.Account = new dataBxaccount()
            {
                account = "40701810700020000087",
                bankcode = "SBR",
                bankname = "Публичное акционерное общество \"Сбербанк России\""
            };
            purchaseRequest.reqagreement = $"Настоящим участник аукциона подтверждает свое согласие поставить товар,выполнить работы и/или оказать услуги на условиях, предусмотренных документацией об электронном аукционе: {lot.Text}, Извещение № {lot.NotificationNumber}.";
            purchaseRequest.reqagreementanswer = "Согласен";
            purchaseRequest.Supplier.opfid = "12247";
            purchaseRequest.Supplier.opfname = "Публичные акционерные общества";
            purchaseRequest.clientinfo = "Browser name: Chrome; Browser version: 71; OS: Windows";
            purchaseRequest.AddDocs(file);
            var xmlResult = _xmlToLower(sberbankXmlService.GetXml(purchaseRequest));
            var sign = await apiDataProvider.SignAsync(xmlResult, ct);
            var viewStateFieldCount = -1;
            int.TryParse(lotDocument.GetValueById("__VIEWSTATEFIELDCOUNT", true), out viewStateFieldCount);

            var form = new Dictionary<string, string>()
                    {
                            { "__EVENTTARGET", lotDocument.GetValueById("__EVENTTARGET",true) },
                            { "__EVENTARGUMENT", lotDocument.GetValueById("__EVENTARGUMENT",true) },
                            { "__VIEWSTATE", lotDocument.GetValueById("__VIEWSTATE",true) },
                            { "__VIEWSTATEGENERATOR", lotDocument.GetValueById("__VIEWSTATEGENERATOR",true) },
                            { "__SCROLLPOSITIONX", "0" },
                            { "__SCROLLPOSITIONY", "2644" },
                            { "ctl00$ctl00$phWorkZone$xmlData", xmlResult },
                            { "ctl00$ctl00$phWorkZone$signValue", sign.Data },
                            { "5",  "Требование" },
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$bxAccount$account",  "40701810700020000087" },
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqAgreement", purchaseRequest.reqagreement},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$signType", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$spFileID", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$ctlHash", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$ctlHash2012", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$ctlSign", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$signed", "True"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDocsPart1$SourceFileID", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$suppBuID", "271203"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$Span3", "12247"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl00", "190200291847"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl01", "Дерипаска Олег Владимирович"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0017", "780401176925"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0119", "Волков Михаил Юрьевич"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0028", "771908391805"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0130", "Галахов Алексей Владимирович"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0039", "774301137338"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0141", "Подгорнова Алла Васильевна"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0050", "773608121576"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0152", "Соломатин Илья Петрович"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0061", "773409188600"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0163", "Власов Алексей Владимирович"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0072", "7702165310"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0174", "Небанковская кредитная организация акционерное   общество  «Национальный расчётный депозитарий»"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0083", "502479276400"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0185", "Коровин Вадим Анатольевич"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0094", "771547468479"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl0196", "Клепиков Алексей Сергеевич"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl00105", "070303559593"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl01107", "Комарова Татьяна Анатольевна"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl00116", "772479309511"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$ctl01118", "Гогленков Александр Владимирович"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDeclarationRequirements", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$reqDeclarationSMP", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$signType", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$spFileID", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$ctlHash", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$ctlHash2012", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$ctlSign", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$signed", "True"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$preferenceDocs$SourceFileID", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$signType", "hash"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$spFileID", file.UploadedFileID},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$ctlHash", file.UploadedHash},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$ctlHash2012", file.UploadedHash2012},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$ctlSign", file.UploadedSign},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$signed", "True"},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2$SourceFileID", ""},
                            { "ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$iptClientInfo", "Browser name: Chrome; Browser version: 71; OS: Windows"},
                            { "ctl00$ctl00$phWorkZone$SignPanel$selectedCertHash",  sign.Fingerprint },
                            { "ctl00$ctl00$phWorkZone$SignPanel$selectedProvider", "RU"},
                            { "ctl00$ctl00$phWorkZone$SignPanel$certList",  sign.Fingerprint },
                            { "ctl00$ctl00$phWorkZone$SignPanel$btnSign", "Подписать"}
                    };

            if (viewStateFieldCount > 0)
            {
                form.Add("__VIEWSTATEFIELDCOUNT", viewStateFieldCount.ToString());
                for (int i = 1; i < viewStateFieldCount; i++)
                    form[$"__VIEWSTATE{i}"] = lotDocument.GetValueById($"__VIEWSTATE{i}", true);
            }

            var formContent = new FormUrlEncodedContent(form);
            var postresult = await httpClientService.PostAsync(lot.Url, formContent, ct);
            _throwIfDocumentError(postresult);

            //purchaseRequest.Docs
            //sberbankXmlService.LoadXML("ctl00_ctl00_phWorkZone_xmlData", "XMLContainer", lotDocument);
            //var eventTarget = lotDocument.GetValueById("__EVENTTARGET");
            //var eventArgument = lotDocument.GetValueById("__EVENTARGUMENT");
            //var viewState = lotDocument.GetValueById("__VIEWSTATE");
            //var date = DateTime.Now;
            //var text = lot.Text;
            //var regNumber = lot.RegNumber;
            //lotDocument.SetValueById("ctl00_ctl00_phWorkZone_phDocumentZone_nbtPurchaseRequest_reqAgreement", configService.GetAgreementText(lot));
            //lotDocument.SetValueById("ctl00_ctl00_phWorkZone_phDocumentZone_nbtPurchaseRequest_reqDeclarationRequirements", configService.GetDeclaration(lot));
            //var FileAttach2tblDoc = lotDocument.GetElementbyId("ctl00$ctl00$phWorkZone$phDocumentZone$nbtPurchaseRequest$FileAttach2tblDoc");
            await logger.Log($"Подача заявки на {lot.NotificationNumber} завершена");
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
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$tbxPurchaseCode" , parameters.NotificationNumber },
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
                error = doc.GetTextById("ctl00_ctl00_phWorkZone_errorMsg", true)?.Trim();
            if (string.IsNullOrEmpty(error))
                error = string.Join("; ", doc.GetValuesByRegexp("<span class=\"errormessage\">(.*?)</span>", true));
            if (string.IsNullOrEmpty(error))
                return;

            throw new Exception(error);
        }

        private void _throwIfDocumentError(string error)
        {
            throw new Exception(error);
        }

        private string _xmlToLower(string xml)
        {
            var tags = new[]
            {
                "purchVersion",
                "notificationFeatures",
                "notificationFeature",
                "placementFeature",
                "featureType",
                "featureTypeName",
                "shortName",
                "maxContractAmount"
            };

            foreach (var tag in tags)
                xml = xml.Replace(tag, tag.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase);

            xml = xml.Replace(@"<?xml version=""1.0""?>
", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        .Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        .Replace(" xsi:type=\"xsd:string\" ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        .Replace("\r\n", " ")
                        .Replace(" />", "/>")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("  ", " ")
                        .Replace("> ", ">"); // Потом отрефакторю, пока неизвестно сработает ли
            //.Replace("  ", string.Empty);

            return xml;
        }

        private async Task<FormUrlEncodedContent> _getAuthStep2Form(HtmlDocument doc, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            var mainContent_xmlData = configService.GetLogonRegisterData(doc.GetTextById("hiddenNow"), doc.GetTextById("hiddenTicket"));
            var sign = await apiDataProvider.SignAsync(mainContent_xmlData, ct);
            var form1Action = doc.GetElementbyId("form1").GetAttributeValue("action", string.Empty).Replace("./", "https://login.sberbank-ast.ru/");
            var result = new Dictionary<string, string>
            {
                { "__EVENTTARGET", doc.GetValueById("__EVENTTARGET") },
                { "__EVENTARGUMENT", doc.GetValueById("__EVENTARGUMENT") },
                { "__VIEWSTATE", doc.GetValueById("__VIEWSTATE") },
                { "__VIEWSTATEGENERATOR", doc.GetValueById("__VIEWSTATEGENERATOR") },
                { "ctl00$mainContent$RadioButtonList2", doc.GetValueById("mainContent_RadioButtonList2_0") },
                { "ctl00$mainContent$txtLoginName", doc.GetValueById("mainContent_txtLoginName") },
                { "ctl00$mainContent$txtPassword", doc.GetValueById("mainContent_txtPassword") },
                { "ctl00$mainContent$DDL1", sign.Fingerprint },
                { "ctl00$mainContent$btnSubmit", doc.GetValueById("mainContent_btnSubmit") },
                { "ctl00$mainContent$xmlData", mainContent_xmlData },
                { "ctl00$mainContent$signValue", sign.Data }
            };

            return new FormUrlEncodedContent(result);
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
