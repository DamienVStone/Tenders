using FtpMonitoringService.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace FtpMonitoringService
{
    public class ZipHelper
    {
        public void ParseArchve(FtpFile root, IEnumerable<ZipArchiveEntry> entries)
        {
            var files = entries.ToDictionary(e => e.FullName.Split(@"\"));
            foreach(var pair in files)
            {
                a(root, pair.Key, pair.Value, 0);
            }
        }

        private void a(FtpFile parent, string[] nameParts, ZipArchiveEntry file, int i)
        {
            if (i >= nameParts.Length || string.IsNullOrWhiteSpace(nameParts[i])) return;
            if (i == nameParts.Length - 1)
            {
                parent.AddChild(nameParts[i], file.Length, file.LastWriteTime);
            }
            else
            {
                a(parent.AddChild(nameParts[i]), nameParts, file, i + 1);
            }
        }
    }
}
