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

            SendNewIndexedFilesUrl = new Uri(configService["api.SendNewIndexedFilesUrl"]);
            GetCurrentIndexUrl = new Uri(configService["api.GetCurrentIndexUrl"]);
            GetUpdatedTenderPlansUrl = new Uri(configService["api.GetUpdatedTenderPlansUrl"]);
            GetNextPathForIndexingUrl = new Uri(configService["api.GetNextPathForIndexingUrl"]);
            GetNextArchiveForMonitoring = new Uri(configService["api.GetNextArchiveForMonitoring"]);
            SendFailedPathNotice = new Uri(configService["api.SendFailedPathNoticeUrl"]);
            SendFailedArchiveNotice = new Uri(configService["api.SendFailedArchiveNoticeUrl"]);
            _sendFilesUrl = configService["api.SendFilesUrl"];
            _sendFileTreeUrl = configService["api.SendFileTreeUrl"];
            _getPathByIdUrl = configService["api.GetPathByIdUrl"];

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

        public Uri SendNewIndexedFilesUrl { get; }
        public Uri GetCurrentIndexUrl { get; }
        public Uri GetUpdatedTenderPlansUrl { get; }
        public Uri GetNextPathForIndexingUrl { get; }
        public Uri GetNextArchiveForMonitoring { get; }
        public Uri SendFailedPathNotice { get; }
        public Uri SendFailedArchiveNotice { get; }

        private string _sendFilesUrl;

        public Uri SendFilesUrl(string pathId)
        {
            return new Uri(_sendFilesUrl + $"?pathId={pathId}");
        }

        private string _sendFileTreeUrl;

        public Uri SendFileTreeUrl(string pathId, string rootId)
        {
            return new Uri(_sendFileTreeUrl + $"?pathId={pathId}&rootId={rootId}");
        }

        private string _getPathByIdUrl;

        public Uri GetPathById(string Id)
        {
            return new Uri(_getPathByIdUrl + $"?pathId={Id}");
        }

        public string SecurityToken { get; }
        public string Username { get; }
        public string Password { get; }
        public bool UseProxy { get; }
        
    }
}
