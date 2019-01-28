using FtpMonitoringService.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
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
    internal class Program
    {
        private static IAPIDataProviderService apiDataProvider;
        private static ILoggerService logger;

        private static void Main(string[] args)
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
            }
            catch (Exception exp)
            {
                await logger.Log("Произошла ошибка в момент получения пути для обработки. Прерываю выполнение.");
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
                var cnt = files.Count();
                await logger.Log($"Найдено {cnt} корневых файлов. Отправляю на сервер");
                if(cnt > 0)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(files), Encoding.UTF8, MediaTypeNames.Application.Json);
                    apiDataProvider.SendFilesAsync(content, p.Id, ct).Wait();
                    await logger.Log("Файлы успешно отправлены");

                    sw.Stop();
                    await logger.Log($"Обработка пути {p.Path} завершена. За {sw.Elapsed}");
                }
                else
                {
                    await logger.Log($"Нет файлов для отправки");
                }
            }
            catch (Exception exp)
            {
                await logger.Log($"Произошла ошибка в момент обработки пути {p.Id}");
                await logger.Log(exp.Message);
                await logger.Log($"Отправляю уведомление об ошибке");
                try
                {
                    await apiDataProvider.SendPathFailedNotice(p.Id, ct);
                }
                catch (Exception e)
                {
                    await logger.Log($"Произошла ошибка при отправке уведомления об ошибке. Продолжаю без уведомления");
                    await logger.Log(e.Message);
                }
            }

            return true;
        }

        private static async Task _startMonitoringArchives(CancellationToken ct)
        {
            await logger.Log($"Начинаю обработку архивов");

#if DEBUG
            var creds = File.ReadAllLines("creds.txt");
#endif
            FTPEntry archive;
            try
            {
                archive = await apiDataProvider.GetNextArchiveForIndexing<FTPEntry>(ct);
            }
            catch (Exception exp)
            {
                await logger.Log("Не удалось получить архив для обработки. Прерываю работу.");
                await logger.Log(exp.Message);
                return;
            }

            while (archive != null)
            {
                try
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
                    await _monitorArchive(file, path, archive.Id, ct);
                }
                catch (Exception exp)
                {
                    await logger.Log($"Произошла ошибка в момент обработки архива {archive.Id}");
                    await logger.Log(exp.Message);
                    await logger.Log($"Отправляю уведомление об ошибке");
                    try
                    {
                        await apiDataProvider.SendArchiveFailedNotice(archive.Id, ct);
                    }
                    catch (Exception e)
                    {
                        await logger.Log($"Произошла ошибка при отправке уведомления об ошибке. Продолжаю без уведомления");
                        await logger.Log(e.Message);
                    }
                }

                try
                {
                    archive = await apiDataProvider.GetNextArchiveForIndexing<FTPEntry>(ct);
                }
                catch (Exception exp)
                {
                    await logger.Log("Не удалось получить архив для обработки. Прерываю работу.");
                    await logger.Log(exp.Message);
                    return;
                }
            }
            await logger.Log($"Все архивы обработаны");
        }

        private static async Task<bool> _monitorArchive(FtpFile f, FtpPath p, string rootId, CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();
            await logger.Log($"Обрабатываю архив {f.Name} по пути {p.Path}");
            var sw1 = new Stopwatch();
            sw1.Start();
            var allEntriesInArchive = FtpClient.Get(logger).GetArchiveEntries(p.Path + f.Name, p.Login, p.Password);
            await logger.Log($"Архив получен {sw1.Elapsed}");
            sw1.Restart();
            new ZipHelper().ParseArchve(f, allEntriesInArchive);
            await logger.Log($"Дерево файлов построено {sw1.Elapsed}");
            sw1.Restart();
            var data = JsonConvert.SerializeObject(f);
            await logger.Log($"Дерево файлов сериализовано {sw1.Elapsed}, отправляю на сервер");
            sw1.Restart();
            var res = await apiDataProvider.SendFileTreeAsync(new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json), p.Id, rootId, ct);
            await logger.Log($"Дерево файлов отправлено на сервер {sw1.Elapsed}");
            sw1.Stop();
            await logger.Log($"Архив {f.Name} обработан ОБЩЕЕ ВРЕМЯ АРХИВА: {sw.Elapsed}");
            sw.Stop();

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
