using System;
using System.Collections.Generic;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Helpers;

namespace Tenders.Integration.API.JobSeeker.Services
{
    public class K8sConfigMapsConfigService : IConfigService
    {
        private object o = new object();
        private Dictionary<string, string> _configs;

        public string this[string index]
        {
            get
            {
                Console.WriteLine("I am is " + ("whoami".Bash()));
                index = index.Replace(".", @"\.");
                Console.WriteLine("try to get " + index);
                if (_configs == null)
                    lock (o)
                        if (_configs == null)
                            _configs = new Dictionary<string, string>();

                if (!_configs.ContainsKey(index))
                    lock (o)
                        if (!_configs.ContainsKey(index))
                        {
                            var result = (@"kubectl get cm api -n tenders -o jsonpath='{.data." + index + "}'").Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                result = (@"kubectl get cm sberbank -n tenders -o jsonpath='{.data." + index + "}'").Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                result = (@"kubectl get cm jobseeker -n tenders -o jsonpath='{.data." + index + "}'").Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                throw new KeyNotFoundException(index);
                            _configs[index] = result;
                        }

                Console.WriteLine("config value got: " + _configs[index]);
                return _configs[index];
            }
        }
    }
}
