using FtpMonitoringService.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
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
            _initContainer();
            logger.Log("Запуск обработки путей.").Wait();
            var cts = new CancellationTokenSource();
            apiDataProvider.Authenticate(cts.Token).Wait();
            while (_doMonitoring(cts.Token).Result) { }
            logger.Log("Завершение обработки путей.").Wait();
        }

        private static async Task<bool> _doMonitoring(CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();
            var p = await apiDataProvider.GetNextPathForIndexing<FtpPath>(ct);
            if (p == null)
            {
                await logger.Log("Нечего обрабатывать.");
                return false;
            }

            List<FtpFile> allEntries = new List<FtpFile>();
#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
            p.Login = creds[0];
            p.Password = creds[1];
#endif
            await logger.Log($"Обработка пути {p.Path}.");
            var files = FtpClient.Get(logger).ListDirectoryFields(p.Path, p.Login, p.Password);
            var filesCount = files.Count();
            await logger.Log($"Найдено {filesCount}");
            var i = 0;
            var notZipFilesToSend = files.Where(f => !f.Name.EndsWith(".zip__")).ToList(); // пока обрабатываем все файлы без погружения
            if (notZipFilesToSend.Count != 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(notZipFilesToSend), Encoding.UTF8, MediaTypeNames.Application.Json);
                apiDataProvider.SendFilesAsync(content, p.Id, ct).Wait();
                i += notZipFilesToSend.Count;
                await logger.Log($"Обработано {i} файлов из {filesCount}");
            }

            files
                .Where(f => f.Name.EndsWith(".zip__"))
                .AsParallel()
                .ForAll(f =>
                {
                    var fs = ZipHelper.Get().ParseArchve(FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password));
                    fs.ForEach(file => file.Parent = f.Id);
                    var res = apiDataProvider.SendFileTreeAsync(new StringContent(JsonConvert.SerializeObject(new { PathId = p.Id, TreeRoot = f, Files = fs }), Encoding.UTF8, MediaTypeNames.Application.Json), ct).Result;
                    logger.Log($"Обработано {i} файлов из {filesCount}").Wait();
                    //logger.Log(res);
                });

            sw.Stop();
            await logger.Log($"Обработка пути {p.Path} завершена.");
            return true;
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
