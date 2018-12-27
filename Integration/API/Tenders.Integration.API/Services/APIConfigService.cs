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
            SetFutureAuctionState = new Uri(configService["api.SetFutureAuctionState"]);
            GetFutureAuction = new Uri(configService["api.GetFutureAuction"]);

            SecurityToken = configService["api.SecurityToken"];
            Username = configService["api.Username"];//
            Password = configService["api.Password"];
            UseProxy = string.Equals(configService["api.UseProxy"], "true", StringComparison.InvariantCultureIgnoreCase);
        }

        public Uri Host { get; }
        public Uri SignText { get; }
        public Uri GetFingerprint { get; }
        public Uri Token { get; }
        public Uri GetProxy { get; }
        public Uri SynchronizeByKey { get; }
        public Uri SetFutureAuctionState { get; }
        public Uri GetFutureAuction { get; }
        public string SecurityToken { get; }
        public string Username { get; }
        public string Password { get; }
        public bool UseProxy { get; }
    }
}
