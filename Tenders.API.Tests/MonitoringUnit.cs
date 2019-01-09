using System.Collections.Generic;

namespace Tenders.API.Tests
{
    public class MonitoringUnit
    {
        public string login { get; set; }
        public string password { get; set; }
        public bool enabled { get; set; }
        public bool isNeedIndex { get; set; }
        public List<string> paths { get; set; }
        public List<string> tempPaths { get; set; }
    }
}
