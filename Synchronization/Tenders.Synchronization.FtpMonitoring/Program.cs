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
            await logger.Log("Получение пути для обработки");
            var p = await apiDataProvider.GetNextPathForIndexing<FtpPath>(ct);
            if (p == null)
            {
                await logger.Log("Нет путей для обаботки, начинаю обработку архивов");
                _startMonitoringArchives();
                return false;
            }

            var sw = new Stopwatch();
            sw.Start();
#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
            p.Login = creds[0];
            p.Password = creds[1];
#endif
            await logger.Log($"Обработка пути {p.Path}.");
            var files = FtpClient.Get(logger).ListDirectoryFiels(p.Path, p.Login, p.Password);
            await logger.Log($"Найдено {files.Count()}. Отправляю на сервер");
            
            var content = new StringContent(JsonConvert.SerializeObject(files), Encoding.UTF8, MediaTypeNames.Application.Json);
            apiDataProvider.SendFilesAsync(content, p.Id, ct).Wait();
            await logger.Log("Файлы успешно отправлены");
            
            sw.Stop();
            await logger.Log($"Обработка пути {p.Path} завершена. За {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            return true;
        }

        private static void _startMonitoringArchives()
        {
            
        }

        private static void _monitorArchive(FtpFile f, FtpPath p)
        {
            
            logger.Log($"Обрабатываю архив {f.Name} по пути {p.Path}");
            var allEntriesInArchive = FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password);
            new ZipHelper().ParseArchve(f, allEntriesInArchive);
            var data = JsonConvert.SerializeObject(f);
            var res = apiDataProvider.SendFileTreeAsync(new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json), p.Id, ct).Result;
            
            
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
