using FtpMonitoringService.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;

namespace FtpMonitoringService
{
    class Program
    {
        private static IAPIDataProviderService apiDataProvider;
        private static ILoggerService logger;

        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            _initContainer();
            _doMonitoring(cts.Token).Wait();
        }

        private static async Task _doMonitoring(CancellationToken ct)
        {
            apiDataProvider.Authenticate(ct).Wait();
            var p = await apiDataProvider.GetNextPathForIndexing<FtpPath>(ct);
            if (p == null) return;

            List<FtpFile> allEntries = new List<FtpFile>();
#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
            p.Login = creds[0];
            p.Password = creds[1];
#endif
            var files = FtpClient.Get(logger).ListDirectoryFiels(p.Path, p.Login, p.Password);
            var notZipFilesToSend = files.Where(f => !f.Name.EndsWith(".zip")).ToList();
            if(notZipFilesToSend.Count != 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(notZipFilesToSend), Encoding.UTF8, MediaTypeNames.Application.Json);
                apiDataProvider.SendFilesAsync(content, p.Id, ct).Wait();
            }
            

            files
                .Where(f => f.Name.EndsWith(".zip"))
                .AsParallel()
                .ForAll(f =>
                {
                    var fs = ZipHelper.Get().ParseArchve(FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password));
                    fs.ForEach(file => file.Parent = f.Id);
                    var res = apiDataProvider.SendFileTreeAsync(new StringContent(JsonConvert.SerializeObject(new { PathId = p.Id, TreeRoot = f, Files = fs }), Encoding.UTF8, MediaTypeNames.Application.Json), ct).Result;
                    logger.Log(res);
                });
        }

        private static void _initContainer()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
#if DEBUG
                sc.AddSingleton<IConfigService, LocalFileConfigService>();
#else
                sc.AddSingleton<IConfigService, EnvironmentVariablesConfigService>();
#endif
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
            });

            apiDataProvider = Container.GetService<IAPIDataProviderService>();
            logger = Container.GetService<ILoggerService>();
        }
    }
}
