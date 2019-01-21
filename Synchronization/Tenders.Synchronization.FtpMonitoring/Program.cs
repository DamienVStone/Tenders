﻿using FtpMonitoringService.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
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
            FtpPath p;
            try
            {
                p = await apiDataProvider.GetNextPathForIndexing<FtpPath>(ct);
            }catch(Exception exp)
            {
                await logger.Log("Произошла ошика в момент получения пути для обработки. Прерываю выполнение.");
                await logger.Log(exp.Message);
                return false;
            }
            
            if (p == null)
            {
                await logger.Log("Нет путей для обаботки, начинаю обработку архивов");
                await _startMonitoringArchives(ct);
                return false;
            }

            try
            {
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

            } catch (Exception exp) {
                await logger.Log($"Произошла ошибка в момент обработки пути {p.Id}");
                await logger.Log(exp.Message);
                await logger.Log($"Отправляю уведомление об ошибке");
                try
                {
                    await apiDataProvider.SendPathFailedNotice(p.Id, ct);
                }catch(Exception e)
                {
                    await logger.Log($"Произошла ошибка при отправке уведомления об ошибке. Продолжаю без уведомления");
                }
            }
            
            return true;
        }

        private async static Task _startMonitoringArchives(CancellationToken ct)
        {
            await logger.Log($"Начинаю обработку архивов");

#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
#endif

            var archive = await apiDataProvider.GetNextArchiveForIndexing<FTPEntry>(ct);
            while(archive != null)
            {
                await logger.Log($"Получил архив {archive.Id}");
                var pathid = archive.Path;
                await logger.Log($"Идентификатор пути {archive.Id}");
                var path = await apiDataProvider.GetPathById<FtpPath>(pathid, ct);
                await logger.Log($"Получил путь {path.Id}");
#if DEBUG
                path.Login = creds[0];
                path.Password = creds[1];
#endif
                var file = new FtpFile(archive.Name, archive.Size, archive.Modified);
                await _monitorArchive(file, path, ct);
                archive = await apiDataProvider.GetNextArchiveForIndexing<FTPEntry>(ct);
            }
            await logger.Log($"Все архивы обработаны");
        }

        private async static Task _monitorArchive(FtpFile f, FtpPath p, CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();
            await logger.Log("Обрабатываю архив {f.Name} по пути {p.Path}");

            var allEntriesInArchive = FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password);
            new ZipHelper().ParseArchve(f, allEntriesInArchive);
            var data = JsonConvert.SerializeObject(f);
            var res = apiDataProvider.SendFileTreeAsync(new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json), p.Id, ct).Result;
            await logger.Log($"Архив {f.Name} обработан {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            sw.Stop();
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
