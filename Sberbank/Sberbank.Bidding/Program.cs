using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Sberbank.Bidding
{
    class Program
    {
        static string Fingerprint;
        static void Main(string[] args)
        {
            Console.WriteLine($"----------ENVIRONMENT VARIABLES---------");
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                string name = (string)env.Key;
                string value = (string)env.Value;
                Console.WriteLine($"{name}={value}");
            }
            Console.WriteLine($"----------------------------------------");

            Helper.Init();
            var ct = new CancellationTokenSource();
            var sw = new Stopwatch();

            sw.Start();
            _auth(ct.Token).ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully)
                    Helper.Logger.Log(t.Exception?.ToString());

                sw.Stop();
                Console.WriteLine("Авторизация заняла " + sw.Elapsed);

            }).Wait(ct.Token);

            sw.Restart();
            _findLot(ct.Token, "0373100123818000053").ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully)
                    Helper.Logger.Log(t.Exception?.ToString());

                sw.Stop();
                Console.WriteLine("Переход на торги занял " + sw.Elapsed);

            });

            while (true) { }
        }

        private static async Task _findLot(CancellationToken ct, string regNumber)
        {
            var client = Helper.Http.GetSberbankClient();
            var doc = new HtmlDocument();

            doc.Load(await Helper.Http.RequestGet(Helper.Constants.SBER_SEARCH_URL, client, ct));

            doc.Load(await Helper.Http.RequestPost(Helper.Constants.SBER_SEARCH_URL, _getSearchForm(doc, regNumber), client, ct));

            var xmlFilterResult = HttpUtility.HtmlDecode(doc.GetElementbyId("ctl00_ctl00_phWorkZone_xmlData")?.InnerText);
            var lots = (data)new XmlSerializer(typeof(data)).Deserialize(new StringReader(xmlFilterResult));
            var link = Helper.Constants.SBER_TRADE_PLACE_URL_TEMPLATE.Replace("{{TRADE_ID}}", lots.row.reqID).Replace("{{ASID}}", lots.row.ASID);
            doc.Load(await Helper.Http.RequestGet(link, client, ct));
            var data = doc.GetElementbyId("phWorkZone_xmlData").GetAttributeValue("value", "Торги еще не проводились или уже завершены");
            Helper.Logger.Log(data);
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
            var ctl00_ctl00_phWorkZone_xmlFilter = Helper.Constants.SBER_SEARCH_TEMPLATE.Replace("{{REG_NUMBER}}", regNumber);
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

        #region auth
        private static async Task _auth(CancellationToken ct)
        {
            var client = Helper.Http.GetSberbankClient();
            var doc = new HtmlDocument();

            var step1Async = Helper.Http.RequestGet(Helper.Constants.SBER_AUTH_STEP1_URL, client, ct);
            var apiAuthAsync = Helper.Api.AuthenticateAsync(ct);
            Task.WaitAll(new[] { step1Async, apiAuthAsync });

            Fingerprint = await Helper.Api.GetFingerprintAsync(ct);
            doc.Load(step1Async.Result);

            doc.Load(await Helper.Http.RequestPost(Helper.Constants.SBER_AUTH_STEP1_URL, _getAuthStep2Form(doc), client, ct));
            doc.Load(await Helper.Http.RequestGet(Helper.Constants.SBER_AUTH_STEP2_URL, client, ct));
            doc.Load(await Helper.Http.RequestPost(Helper.Constants.SBER_AUTH_STEP3_URL, _getAuthStep3Form(doc), client, ct));

            ct.ThrowIfCancellationRequested();
            Helper.Logger.Log("Авторизован как: " + doc.GetElementbyId("ctl00_loginctrl_link").InnerText.Trim());
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
            var mainContent_xmlData = Helper.Constants.SBER_LOGON_REGISTER_TEXT
                .Replace("{{NOW}}", hiddenNow)
                .Replace("{{TICKET}}", hiddenTicket);
            var mainContent_DDL1 = Fingerprint;

            var mainContent_signValue = Helper.Api.SignAsync(mainContent_xmlData, new CancellationToken()).Result;

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
