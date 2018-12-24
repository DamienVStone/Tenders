using System;
using System.Collections;
using System.Collections.Generic;
using Tenders.Core.Abstractions.Services;

namespace Tenders.Core.Services
{
    public class EnvironmentVariablesConfigService : IConfigService
    {
        private readonly ILoggerService loggerService;
        private object o = new object();
        private Dictionary<string, string> _configs;

        public EnvironmentVariablesConfigService(
            ILoggerService loggerService
            )
        {
            this.loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
        }

        public string this[string index]
        {
            get
            {
                if (_configs == null)
                    lock (o)
                        if (_configs == null)
                        {
                            loggerService.Log($"***************Переменные окружения:*****************");
                            foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
                            {
                                string name = (string)env.Key;
                                string value = (string)env.Value;
                                loggerService.Log($"{name}={value}");
                            }
                            loggerService.Log($"****************************************************");
                        }

                return _configs[index];
            }
        }
    }
}
