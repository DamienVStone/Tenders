using HtmlAgilityPack;
using Newtonsoft.Json;
using Sberbank.Bidding.Helpers;
using Sberbank.Bidding.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Sberbank.Bidding
{
    class Program
    {
        static string Fingerprint;
        static int Main(string[] args)
        {
            "----------ENVIRONMENT VARIABLES---------".Log();
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                string name = (string)env.Key;
                string value = (string)env.Value;
                Console.WriteLine($"{name}={value}");
            }
            "----------------------------------------".Log();

            Http.Init();
            var ct = new CancellationTokenSource();
            var sw = new Stopwatch();

            var auction = _getFutureAuction(ct.Token).Result;
            if (auction == null)
                "Нет доступных аукционов.".Log();
            else
            {
                $"Обработка аукциона {auction.Code} начало торгов {auction.StartTime}".Log();
                sw.Start();
                _auth(ct.Token).ContinueWith(t =>
                {
                    if (!t.IsCompletedSuccessfully) t.Exception?.Log();

                    sw.Stop();
                    $"Авторизация заняла {sw.Elapsed}".Log();

                }).Wait(ct.Token);

                ct.Token.ThrowIfCancellationRequested();

                sw.Restart();
                var found = _findLot(ct.Token, auction.Code).ContinueWith(t =>
                {
                    if (!t.IsCompletedSuccessfully) t.Exception?.Log();

                    sw.Stop();
                    $"Поиск лота занял {sw.Elapsed}".Log();
                    return t.Result;
                }).Result;

                ct.Token.ThrowIfCancellationRequested();

                var doc = _moveToTradePlace(found, ct.Token).ContinueWith(t =>
                {
                    if (!t.IsCompletedSuccessfully) t.Exception?.Log();

                    sw.Stop();
                    $"Переход на торги занял {sw.Elapsed}".Log();
                    return t.Result;
                }).Result;

                return 0;

                ct.Token.ThrowIfCancellationRequested();

                while (true)
                {
                    Api.SyncronizeByKey(ct.Token).Wait();
                    _bid(auction, found, doc, ct.Token).ContinueWith(t =>
                    {
                        if (!t.IsCompletedSuccessfully) t.Exception?.Log();

                        sw.Stop();
                        $"Подача предложения заняла {sw.Elapsed}".Log();
                    });

                    //Http.GetApiClient().GetAsync(Constants.API_GET_PROXY_URL.Replace("GetPayedProxy", "GetIP"));
                }
            }
            return 0;
        }

        private static async Task<HtmlDocument> _moveToTradePlace(data foundAuction, CancellationToken ct)
        {
            var doc = new HtmlDocument();
            var link = Constants.SBER_TRADE_PLACE_URL_TEMPLATE.Replace("{{TRADE_ID}}", foundAuction.row.reqID).Replace("{{ASID}}", foundAuction.row.ASID);
            doc.Load(await Http.RequestGet(link, Http.GetSberbankClient(), ct));
            var data = doc.GetElementbyId("phWorkZone_xmlData").GetAttributeValue("value", "Торги еще не проводились или уже завершены");
            Logger.Log(data);
            return doc;
        }

        private static async Task _bid(FutureAuction fa, data foundAuction, HtmlDocument doc, CancellationToken ct)
        {
            var requestNo = WebUtility.HtmlDecode(doc.GetElementbyId("requestNo").GetAttributeValue("value", ""));
            var priceSign = WebUtility.HtmlDecode(doc.GetElementbyId("priceSign").GetAttributeValue("value", ""));
            var supplierName = WebUtility.HtmlDecode(doc.GetElementbyId("supplierName").GetAttributeValue("value", ""));
            var data = Constants.SBER_BID_DATA_TEMPLATE
                .Replace("{{MYNEWPRICE}}", fa.MinCost.ToString().Replace(",", "."))
                .Replace("{{REQID}}", foundAuction.row.reqID)
                .Replace("{{REQUESTNO}}", requestNo)
                .Replace("{{PRICESIGN}}", priceSign)
                .Replace("{{PURCHASEID}}", foundAuction.row.purchID)
                .Replace("{{PURCHASECODE}}", foundAuction.row.purchCode)
                .Replace("{{PURCHASENAME}}", foundAuction.row.purchName)
                .Replace("{{SUPPLIERNAME}}", supplierName);
            var hash = await data.SignAsync(ct);

            var client = Http.GetSberbankClient();
            var bid = new Bid
            {
                reqID = foundAuction.row.reqID,
                xmlData = data,
                Hash = hash
            };

            var jsonData = JsonConvert.SerializeObject(bid);
            var jsonContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var postUrl = Constants.SBER_TRADE_PLACE_BID_URL_TEMPLATE.Replace("{{TRADE_ID}}", foundAuction.row.reqID).Replace("{{ASID}}", foundAuction.row.ASID);
            doc.Load(await Http.RequestPost(postUrl, jsonContent, Http.GetSberbankClient(), ct));
            doc.Text.Log();
        }

        #region move to lot
        private static async Task<FutureAuction> _getFutureAuction(CancellationToken ct)
        {
            await Api.AuthenticateAsync(ct);
            return await Api.GetFutureAuctionAsync(ct);
        }

        private static async Task<data> _findLot(CancellationToken ct, string regNumber)
        {
            var client = Http.GetSberbankClient();
            var doc = new HtmlDocument();

            doc.Load(await Http.RequestGet(Constants.SBER_SEARCH_URL, client, ct));

            doc.Load(await Http.RequestPost(Constants.SBER_SEARCH_URL, _getSearchForm(doc, regNumber), client, ct));

            var xmlFilterResult = HttpUtility.HtmlDecode(doc.GetElementbyId("ctl00_ctl00_phWorkZone_xmlData")?.InnerText);
            var lots = (data)new XmlSerializer(typeof(data)).Deserialize(new StringReader(xmlFilterResult));
            return lots;
        }

        private static FormUrlEncodedContent _getSearchForm(HtmlDocument doc, string regNumber)
        {
            var _EVENTTARGET = doc.GetElementbyId("_EVENTTARGET")?.GetAttributeValue("value", string.Empty);
            var __EVENTARGUMENT = doc.GetElementbyId("__EVENTARGUMENT")?.GetAttributeValue("value", string.Empty);
            var __VIEWSTATEFIELDCOUNT = doc.GetElementbyId("__VIEWSTATEFIELDCOUNT")?.GetAttributeValue("value", string.Empty);
            var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE")?.GetAttributeValue("value", string.Empty);
            var __VIEWSTATE1 = doc.GetElementbyId("__VIEWSTATE1")?.GetAttributeValue("value", string.Empty);
            var __VIEWSTATE2 = doc.GetElementbyId("__VIEWSTATE2")?.GetAttributeValue("value", string.Empty);
            var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR")?.GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_xmlFilter = Constants.SBER_SEARCH_TEMPLATE.Replace("{{REG_NUMBER}}", regNumber);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbxPurchaseCode = regNumber;
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbSearch = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbSearch").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountstart = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountstart").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountend = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountend").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateStart = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateStart").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateEnd = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateEnd").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRRequestDateStart = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRRequestDateStart").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRequestDateEnd = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRequestDateEnd").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateStart = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateStart").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateEnd = doc.GetElementbyId("ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateEnd").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_btnSearch = doc.GetElementbyId("ctl00_ctl00_phWorkZone_btnSearch").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_hfFilterZone = doc.GetElementbyId("ctl00_ctl00_phWorkZone_hfFilterZone").GetAttributeValue("value", string.Empty);
            var ctl00_ctl00_phWorkZone_xmlData = doc.GetElementbyId("ctl00_ctl00_phWorkZone_xmlData").GetAttributeValue("value", string.Empty);

            var values = new Dictionary<string, string>()
            {
                { "_EVENTTARGET" , _EVENTTARGET },
                { "__EVENTARGUMENT" , __EVENTARGUMENT },
                { "__VIEWSTATEFIELDCOUNT" , __VIEWSTATEFIELDCOUNT },
                { "__VIEWSTATE" , __VIEWSTATE },
                { "__VIEWSTATE1" , __VIEWSTATE1 },
                { "__VIEWSTATE2" , __VIEWSTATE2 },
                { "__VIEWSTATEGENERATOR" , __VIEWSTATEGENERATOR },
                { "ctl00$ctl00$phWorkZone$xmlFilter" , ctl00_ctl00_phWorkZone_xmlFilter },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$tbxPurchaseCode" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbxPurchaseCode },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$tbSearch" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_tbSearch },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$purchamountstart" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountstart },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$purchamountend" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_purchamountend },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$cldPublicDateStart" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateStart },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$cldPublicDateEnd" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_cldPublicDateEnd },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldRRequestDateStart" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRRequestDateStart },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldRequestDateEnd" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldRequestDateEnd },
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldAuctionBeginDateStart" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateStart},
                { "ctl00$ctl00$phWorkZone$phFilterZone$nbtPurchaseListFilter$CldAuctionBeginDateEnd" , ctl00_ctl00_phWorkZone_phFilterZone_nbtPurchaseListFilter_CldAuctionBeginDateEnd },
                { "ctl00$ctl00$phWorkZone$btnSearch" , ctl00_ctl00_phWorkZone_btnSearch },
                { "ctl00$ctl00$phWorkZone$hfFilterZone" , ctl00_ctl00_phWorkZone_hfFilterZone },
                { "ctl00$ctl00$phWorkZone$xmlData" , ctl00_ctl00_phWorkZone_xmlData }
            };

            return new FormUrlEncodedContent(values);
        }
        #endregion

        #region auth
        private static async Task _auth(CancellationToken ct)
        {
            var client = Http.GetSberbankClient();
            var doc = new HtmlDocument();

            Fingerprint = await Api.GetFingerprintAsync(ct);
            doc.Load(await Http.RequestGet(Constants.SBER_AUTH_STEP1_URL, client, ct));

            doc.Load(await Http.RequestPost(Constants.SBER_AUTH_STEP1_URL, _getAuthStep2Form(doc), client, ct));
            doc.Load(await Http.RequestGet(Constants.SBER_AUTH_STEP2_URL, client, ct));
            doc.Load(await Http.RequestPost(Constants.SBER_AUTH_STEP3_URL, _getAuthStep3Form(doc), client, ct));

            ct.ThrowIfCancellationRequested();
            Logger.Log("Авторизован как: " + doc.GetElementbyId("ctl00_loginctrl_link").InnerText.Trim());
        }

        private static FormUrlEncodedContent _getAuthStep2Form(HtmlDocument doc)
        {
            var __VIEWSTATE = doc.GetElementbyId("__VIEWSTATE").GetAttributeValue("value", string.Empty);
            var __VIEWSTATEGENERATOR = doc.GetElementbyId("__VIEWSTATEGENERATOR").GetAttributeValue("value", string.Empty);
            var mainContent_RadioButtonList2_0 = doc.GetElementbyId("mainContent_RadioButtonList2_0").GetAttributeValue("value", string.Empty);
            var mainContent_txtLoginName = doc.GetElementbyId("mainContent_txtLoginName").GetAttributeValue("value", string.Empty);
            var mainContent_txtPassword = doc.GetElementbyId("mainContent_txtPassword").GetAttributeValue("value", string.Empty);
            var hiddenNow = doc.GetElementbyId("hiddenNow").InnerText;
            var hiddenTicket = doc.GetElementbyId("hiddenTicket").InnerText;
            var form1Action = doc.GetElementbyId("form1").GetAttributeValue("action", string.Empty).Replace("./", "https://login.sberbank-ast.ru/");
            var mainContent_xmlData = Constants.SBER_LOGON_REGISTER_TEXT
                .Replace("{{NOW}}", hiddenNow)
                .Replace("{{TICKET}}", hiddenTicket);
            var mainContent_DDL1 = Fingerprint;

            var mainContent_signValue = Api.SignAsync(mainContent_xmlData, new CancellationToken()).Result;

            return new FormUrlEncodedContent(
                new Dictionary<string, string>()
                {
                            { "__VIEWSTATE", __VIEWSTATE },
                            { "__VIEWSTATEGENERATOR", __VIEWSTATEGENERATOR },
                            { "ctl00$mainContent$RadioButtonList2", mainContent_RadioButtonList2_0 },
                            { "ctl00$mainContent$txtLoginName", mainContent_txtLoginName },
                            { "ctl00$mainContent$txtPassword", mainContent_txtPassword },
                            { "ctl00$mainContent$DDL1", mainContent_DDL1 },
                            { "ctl00$mainContent$xmlData", mainContent_xmlData },
                            { "ctl00$mainContent$signValue", mainContent_signValue }
                });
        }

        private static FormUrlEncodedContent _getAuthStep3Form(HtmlDocument doc)
        {
            var wa = doc.DocumentNode.SelectSingleNode("//*[@name='wa']").GetAttributeValue("value", string.Empty);
            var wresult = doc.DocumentNode.SelectSingleNode("//*[@name='wresult']").GetAttributeValue("value", string.Empty);
            var StsCertsChecked = doc.DocumentNode.SelectSingleNode("//*[@name='StsCertsChecked']").GetAttributeValue("value", string.Empty);
            var values = new Dictionary<string, string>()
                    {
                            { "wa", wa },
                            { "wresult", HttpUtility.HtmlDecode(wresult) },
                            { "StsCertsChecked", StsCertsChecked },
                    };

            return new FormUrlEncodedContent(values);
        }
        #endregion
    }
}
