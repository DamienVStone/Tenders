using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
            Console.WriteLine("proxy is: " + Helper.Http.Proxy.Address);
            while (true) { }
            var ct = new CancellationTokenSource();
            var sw = new Stopwatch();
            sw.Start();
            _auth(ct.Token).ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully)
                    Helper.Logger.Log(t.Exception?.ToString());

                sw.Stop();
                Console.WriteLine("Авторизация заняла " + sw.Elapsed);

            });

            Console.ReadKey();
        }

        private static async Task _auth(CancellationToken ct)
        {
            var client = Helper.Http.GetSberbankClient();
            var doc = new HtmlDocument();

            var step1Async = Helper.Http.RequestGet(new Uri(Helper.Constants.SBER_AUTH_STEP1_URL), client, ct);
            var apiAuthAsync = Helper.Api.AuthenticateAsync(ct);
            Task.WaitAll(new[] { step1Async, apiAuthAsync });

            Fingerprint = await Helper.Api.GetFingerprintAsync(ct);
            doc.Load(step1Async.Result);
            doc.Load(await Helper.Http.RequestPost(new Uri(Helper.Constants.SBER_AUTH_STEP1_URL), _getAuthStep2Form(doc), client, ct));
            doc.Load(await Helper.Http.RequestGet(new Uri(Helper.Constants.SBER_AUTH_STEP2_URL), client, ct));
            doc.Load(await Helper.Http.RequestPost(new Uri(Helper.Constants.SBER_AUTH_STEP3_URL), _getAuthStep3Form(doc), client, ct));

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
            var mainContent_DDL1 = string.Empty;
            mainContent_DDL1 = Fingerprint;

            var mainContent_signValue = string.Empty;
            mainContent_signValue = Helper.Api.SignAsync(mainContent_xmlData, new CancellationToken()).Result.Replace("\\r\\n", "\r\n").Replace("\"", string.Empty);

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
    }
}
