using System;

namespace AppLogger.Models
{
    public class LogEntry
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public LogType LogType { get; set; }
    }
}
