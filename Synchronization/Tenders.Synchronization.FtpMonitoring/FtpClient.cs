using FtpMonitoringService.Models;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FtpMonitoringService
{
    public class FtpClient
    {
        public static FtpClient Get()
        {
            return new FtpClient();
        }

        private FtpClient() {}

        public FtpFile[] ListDirectoryFiels(string dirPath, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
            request.Credentials = new NetworkCredential(username, password);
#if DEBUG
            request.EnableSsl = false;
#else
            request.EnableSsl = true;
#endif
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            string responseText;
            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseBody = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(responseBody);
                responseText = Encoding.Default.GetString(responseBody.ToArray());
            };

            return responseText.Split("\r\n").Where(l => !string.IsNullOrEmpty(l)).Select(l => _lineToFile(dirPath, l)).Where(f => !f.IsDirectory).ToArray();
        }

        public ZipArchiveEntry[] GetArchiveEntries(string filePath, string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
            request.Credentials = new NetworkCredential(username, password);
#if DEBUG
            request.EnableSsl = false;
#else
            request.EnableSsl = true;
#endif
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            ZipArchiveEntry[] entries;
            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseBody = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(responseBody);
                var archive = new ZipArchive(responseBody, ZipArchiveMode.Read);
                entries = new ZipArchiveEntry[archive.Entries.Count];
                archive.Entries.CopyTo(entries, 0);
            }

            return entries;
        }

        private FtpFile _lineToFile(string parentDir, string lineToFile)
        {
            var parts = Regex.Replace(lineToFile, @"\s+", " ").Split(" ");
            var file = new FtpFile();
            file.Name = parts[3];
            file.DateModified = DateTime.ParseExact(parts[0] + " " + parts[1], "MM-dd-yy hh:mmtt", CultureInfo.GetCultureInfoByIetfLanguageTag("en"));
            if (parts[2].Equals("<DIR>"))
            {
                file.IsDirectory = true;
                file.Name += "/";
            }
            else
            {
                file.Size = long.Parse(parts[2]);
            }
            return file;
        }

    }
}
