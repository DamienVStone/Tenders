using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tenders.Core.Abstractions.Services;

namespace Tenders.Core.Services
{
    public class ConfigService : IConfigService
    {
        private object o = new object();
        private Dictionary<string, string> _configs;

        public string this[string index]
        {
            get
            {
                if (_configs == null)
                    lock (o)
                        if (_configs == null)
                            _configs = File
                                .ReadAllLines(@"C:\temp\configs.txt")
                                .Where(c => !string.IsNullOrEmpty(c?.Trim()))
                                .Select(s => s.Split("=", 2))
                                .ToDictionary(p => p[0], p => p[1]);

                return _configs[index];
            }
        }
    }
}
