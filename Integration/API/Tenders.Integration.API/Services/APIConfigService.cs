using System;
using Tenders.Core.Abstractions.Services;
using Tenders.Integration.API.Interfaces;

namespace Tenders.Integration.API.Services
{
    public class APIConfigService : IAPIConfigService
    {
        public APIConfigService(IConfigService configService)
        {
            if (configService == null)
                throw new ArgumentNullException(nameof(configService));

            Host = new Uri(configService["api.Host"]);
            SignText = new Uri(Host + configService["api.SignTextUrl"]);
            GetFingerprint = new Uri(Host + configService["api.GetFingerprintUrl"]);
            Token = new Uri(Host + configService["api.TokenUrl"]);
            GetProxy = new Uri(configService["api.GetProxyUrl"]);
            SynchronizeByKey = new Uri(configService["api.SynchronizeByKeyUrl"]);

            SecurityToken = configService["api.SecurityToken"];
            Username = configService["api.Username"];
            Password = configService["api.Password"];
            UseProxy = string.Equals(configService["api.UseProxy"], "true", StringComparison.InvariantCultureIgnoreCase);

            DebugProxyHost = configService["debug.proxy.host"];
            DebugProxyUsername = configService["debug.proxy.username"];
            DebugProxyPassword = configService["debug.proxy.password"];
        }

        public Uri Host { get; }
        public Uri SignText { get; }
        public Uri GetFingerprint { get; }
        public Uri Token { get; }
        public Uri GetProxy { get; }
        public Uri SynchronizeByKey { get; }
        public string SecurityToken { get; }
        public string Username { get; }
        public string Password { get; }
        public bool UseProxy { get; }
        public string DebugProxyHost { get; }
        public string DebugProxyUsername { get; }
        public string DebugProxyPassword { get; }
    }
}
