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
            await logger.Log("Получение пути для обработки");
            var p = await apiDataProvider.GetNextPathForIndexing<FtpPath>(ct);
            if (p == null)
            {
                await logger.Log("Нечего обрабатывать.");
                return false;
            }
            
#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
            p.Login = creds[0];
            p.Password = creds[1];
#endif
            await logger.Log($"Обработка пути {p.Path}.");
            var sw1 = new Stopwatch();
            sw1.Start();
            var files = FtpClient.Get(logger).ListDirectoryFiels(p.Path, p.Login, p.Password);
            var filesCount = files.Count();
            sw1.Stop();
            await logger.Log($"Получаю список файлов по пути {sw1.Elapsed.Minutes}:{sw1.Elapsed.Seconds}");
            await logger.Log($"Найдено {filesCount}");
            var key = new object();
            var i = 0;
            var notZippedTask = Task.Run(() =>
            {
                var sw2 = new Stopwatch();
                sw2.Start();
                var notZipFilesToSend = files.Where(f => !f.Name.EndsWith(".zip")); // пока обрабатываем все файлы без погружения
                if (notZipFilesToSend.Count() != 0)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(notZipFilesToSend), Encoding.UTF8, MediaTypeNames.Application.Json);
                    apiDataProvider.SendFilesAsync(content, p.Id, ct).Wait();
                    lock(key) i += notZipFilesToSend.Count();
                    
                }
                sw2.Stop();
                logger.Log($"Отправляю не архивные корневые файлы {sw2.Elapsed.Minutes}:{sw2.Elapsed.Seconds}").Wait();
                logger.Log($"Обработано {i} файлов из {filesCount}").Wait();
            });

            files
                .Where(f => f.Name.EndsWith(".zip"))
                .AsParallel()
                .ForAll(f =>
                {
                    var sww = new Stopwatch();
                    sww.Start();
                    logger.Log($"Обрабатываю архив {f.Name} по пути {p.Path}");
                    var allEntriesInArchive = FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password);
                    logger.Log($"Получил список элементов архива {sww.Elapsed.Minutes}:{sww.Elapsed.Seconds}");
                    var sww1 = new Stopwatch();
                    sww1.Start();
                    new ZipHelper().ParseArchve(f, allEntriesInArchive);
                    logger.Log($"Построил дерево детей из архива {sww1.Elapsed.Minutes}:{sww1.Elapsed.Seconds}");
                    sww1.Restart();
                    var data = JsonConvert.SerializeObject(f);
                    logger.Log($"Сериализую дерево детей {sww1.Elapsed.Minutes}:{sww1.Elapsed.Seconds}");
                    sww1.Restart();
                    var res = apiDataProvider.SendFileTreeAsync(new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json), p.Id, ct).Result;
                    logger.Log($"Отправляю на сервер {sww1.Elapsed.Minutes}:{sww1.Elapsed.Seconds}");
                    sww1.Stop();
                    lock (key) i++;
                    logger.Log($"Обработал архив {f.Name} {sww.Elapsed.Minutes}:{sww.Elapsed.Seconds}");
                    sww.Stop();
                    logger.Log($"Обработано {i} файлов из {filesCount}").Wait();
                });

            notZippedTask.Wait();

            sw.Stop();
            await logger.Log($"Обработка пути {p.Path} завершена. За {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
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
