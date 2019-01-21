using System;

namespace FtpMonitoringService.Models
{
    public class FTPEntry
    {
        public string Id { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string State { get; set; }
        public string Path { get; set; }
        public string Parent { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsArchive { get; set; }
    }
}
