using FtpMonitoringService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Tenders.Core.Abstractions.Services;

namespace FtpMonitoringService
{
    public class FtpClient
    {
        private readonly ILoggerService logger;

        public static FtpClient Get(ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            return new FtpClient(logger);
        }

        private FtpClient(ILoggerService logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<FtpFile> ListDirectoryFiels(string dirPath, string username, string password)
        {
            string responseText;
            using (var responseBody = new MemoryStream())
            {
                _downloadFromFtp(dirPath, username, password, responseBody, WebRequestMethods.Ftp.ListDirectoryDetails);
                responseText = Encoding.Default.GetString(responseBody.ToArray());
            };

            return responseText.Split("\r\n").Where(l => !string.IsNullOrEmpty(l)).Select(l => _lineToFile(dirPath, l)).Where(f => !f.IsDirectory);
        }

        public ISet<ZipArchiveEntry> GetArchiveEntries(string filePath, string username, string password)
        {
            ISet<ZipArchiveEntry> entries;
            var sw = new Stopwatch();
            using (var responseBody = new MemoryStream())
            {
                _downloadFromFtp(filePath, username, password, responseBody, WebRequestMethods.Ftp.DownloadFile);
                sw.Start();
                var archive = new ZipArchive(responseBody, ZipArchiveMode.Read);
                logger.Log($"Распаковываю архив {sw.Elapsed}");
                sw.Restart();
                entries = new HashSet<ZipArchiveEntry>();
                entries.UnionWith(archive.Entries);
                logger.Log($"Кладу в выходную коллекцию {sw.Elapsed}");
            }
            sw.Stop();
            return entries;
        }

        private void _downloadFromFtp(string path, string username, string password, Stream copyHere, string method)
        {
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(username, password);
            request.EnableSsl = false;
            request.Method = method;
            FtpWebResponse response = null;
            while (true)
            {
                try
                {
                    response = (FtpWebResponse)request.GetResponse();
                    response.GetResponseStream().CopyTo(copyHere);
                    return;
                }
                catch (Exception exp)
                {
                    logger.Log("Произошла ошибка в процессе запроса к FTP").Wait();
                    logger.Log(exp.Message).Wait();
                    logger.Log("Жду одну секунду").Wait();
                    Thread.Sleep(1000);
                    logger.Log("Повторяю запрос").Wait();
                }
                finally
                {
                    if (response != null) response.Dispose();
                }
            }
        }

        private FtpFile _lineToFile(string parentDir, string lineToFile)
        {
#if DEBUG
            var parts = Regex.Replace(lineToFile, @"\s+", " ").Split(" ");

            var name = parts[3];
            if (parts[2].Equals("<DIR>"))
            {
                return new FtpFile(name);
            }
            else
            {
                var modified = DateTimeOffset.ParseExact(parts[0] + " " + parts[1], "MM-dd-yy hh:mmtt", CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
                var size = long.Parse(parts[2]);
                return new FtpFile(name, size, modified);
            }
#else
            var isDir = lineToFile.StartsWith("d");
            var cutted = lineToFile.Substring(29).Trim();
            var parts = cutted.Split(' ');

            var name = parts.Last() + (isDir ? "/" : "");
            if (isDir)
            {
                return new FtpFile(name);
            }
            else
            {
                var modified = _parseDate(parts);
                var size = long.Parse(parts[0]);

                // Для уверенности проверим полученную дату
                if (modified > DateTime.Now)
                    logger.Log($"Дата изменения файла {name} - {modified} больше текущей!");

                return new FtpFile(name, size, modified);
            }
#endif
        }

#if !DEBUG
        private DateTime _parseDate(string[] parts)
        {
            var isContainsTime = parts.Length < 6;
            var year = isContainsTime ? $"{DateTime.Now.Year} {parts[3]}" : parts[4]; // Добавляем год если указано только время
            var changeDate = DateTime.Parse($"{parts[1]} {parts[2]} {year}", CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
            // Если месяц больше текущего - значит это данные данные за прошлый год (догадка)
            if (changeDate.Month > DateTime.Now.Month)
                changeDate = changeDate.AddYears(-1);

            return changeDate;
        }
#endif

    }
}
