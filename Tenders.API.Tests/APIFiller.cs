using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using TenderPlanAPI.Parameters;

namespace Tenders.API.Tests
{
    [TestClass]
    public class APIFiller
    {
        [TestMethod]
        public void Parse()
        {
            var creds = File.ReadAllLines("creds.dat");
            
            var data = File.ReadAllText("data.json");
            var monitoringUnits = JsonConvert.DeserializeObject<List<MonitoringUnit>>(data);

            Debug.Print("Начал параллельную обработку путей");
            monitoringUnits
                .AsParallel()
                .ForAll(s => {
                    sendAll(s, s.paths, creds);
                    sendAll(s, s.tempPaths, creds);
                });
        }

        private void sendOne(FTPPathParam path, string[] creds)
        {
            var proxy = new WebProxy("http://mwg.corp.ingos.ru:9090", true);
            proxy.Credentials = new NetworkCredential() { UserName = creds[0], Password = creds[1] };
            var handler = new HttpClientHandler() { Proxy = proxy, UseProxy = true };
            using (var client = new HttpClient(handler))
            {
                
                var content = new StringContent(JsonConvert.SerializeObject(path), Encoding.UTF8, MediaTypeNames.Application.Json);
                var res = client.PostAsync("http://5.8.180.100:30020/api/FtpPath", content).Result;
                Debug.Print($"Отправил: {path.Path}");
            }
        }

        private void sendAll(MonitoringUnit s, IEnumerable<string> paths, string[] creds)
        {
            paths.AsParallel()
                        .ForAll(p =>
                            sendOne(new FTPPathParam()
                            {
                                Id = ObjectId.GenerateNewId(),
                                Password = s.password,
                                Login = s.login,
                                Path = "ftp://"+p
                            }, creds)
                        );
        }

    }
}
