using AppLogger.Models;
using System.Collections.Generic;

namespace AppLogger.Configuration
{
    class LoggerConfigSection
    {
        public string ApiUrl { get; set; }
        public LogType LogLevel { get; set; }
        public Timeout Timeout { get; set; }
        public List<LoggerConfig> Loggers { get; set; }
    }

    class Timeout {
        public long? Log { get; set; }
        public long? Status { get; set; }
        public long? Http { get; set; }
    }

    class LoggerConfig {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
