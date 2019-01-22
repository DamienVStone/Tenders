using Newtonsoft.Json;
using Tenders.Core.Abstractions.Services;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;

namespace Tenders.Sberbank.Services
{
    public class SberbankConfigService : ISberbankConfigService
    {
        public string AuthStep1Url { get; private set; }
        public string AuthStep2Url { get; private set; }
        public string AuthStep3Url { get; private set; }
        public string PurchaseRequestListUrl { get; private set; }
        public bool IsDebug { get; private set; }
        public IAuctionInfo AuctionInfo { get; private set; }
        public string UploadFileUrl { get; private set; }

        private string _logonRegisterDataTemplate;
        private string _searchXmlTemplate;

        private string _tradePlaceUrlTemplate;
        private string _tradePlaceBidUrlTemplate;
        private string _bidDataTemplate;
        private string _asyncRefreshDataTemplate;

        public SberbankConfigService(
            IConfigService configService
            )
        {
            if (configService == null)
                throw new System.ArgumentNullException(nameof(configService));

            AuctionInfo = JsonConvert.DeserializeObject<AuctionInfo>(configService["sberbank.AuctionInfo"]);
            AuthStep1Url = configService["sberbank.AuthStep1Url"];
            AuthStep2Url = configService["sberbank.AuthStep2Url"];
            AuthStep3Url = configService["sberbank.AuthStep3Url"];
            UploadFileUrl = configService["sberbank.UploadFileUrl"];
            PurchaseRequestListUrl = configService["sberbank.PurchaseRequestListUrl"];
            _logonRegisterDataTemplate = configService["sberbank.LogonRegisterDataTemplate"];
            _searchXmlTemplate = configService["sberbank.SearchXmlTemplate"];
            _tradePlaceUrlTemplate = configService["sberbank.TradePlaceUrlTemplate"];
            _tradePlaceBidUrlTemplate = configService["sberbank.TradePlaceBidUrlTemplate"];
            _bidDataTemplate = configService["sberbank.BidDataTemplate"];
            _asyncRefreshDataTemplate = configService["sberbank.AsyncRefreshDataTemplate"];

            IsDebug = string.Equals(configService["sberbank.debug"], "true", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetLogonRegisterData(string timeNow, string ticket)
        {
            if (string.IsNullOrEmpty(timeNow))
                throw new System.ArgumentException(nameof(timeNow));

            if (string.IsNullOrEmpty(ticket))
                throw new System.ArgumentException(nameof(ticket));

            return _logonRegisterDataTemplate
                .Replace("{{NOW}}", timeNow)
                .Replace("{{TICKET}}", ticket);
        }

        public string GetSearchXml(ISearchParameters parameters)
        {
            return _searchXmlTemplate.Replace("{{REG_NUMBER}}", parameters.Regnumber);
        }

        public string GetTradePlaceUrl(string tradeId, string asid)
        {
            return _tradePlaceUrlTemplate
                .Replace("{{TRADE_ID}}", tradeId)
                .Replace("{{ASID}}", asid);
        }

        public string GetTradePlaceBidUrl(string tradeId, string asid)
        {
            return _tradePlaceBidUrlTemplate
                .Replace("{{TRADE_ID}}", tradeId)
                .Replace("{{ASID}}", asid);
        }

        public string GetBidData(decimal price, ITradePlace tradePlace)
        {
            if (tradePlace == null)
            {
                throw new System.ArgumentNullException(nameof(tradePlace));
            }

            return _bidDataTemplate
                .Replace("{{MYNEWPRICE}}", price.ToString().Replace(",", "."))
                .Replace("{{REQID}}", tradePlace.ReqID)
                .Replace("{{REQUESTNO}}", tradePlace.ReqNo)
                .Replace("{{PRICESIGN}}", tradePlace.PriceSign)
                .Replace("{{PURCHASEID}}", tradePlace.PurchId)
                .Replace("{{PURCHASECODE}}", tradePlace.PurchCode)
                .Replace("{{PURCHASENAME}}", tradePlace.PurchName)
                .Replace("{{SUPPLIERNAME}}", tradePlace.SuppName);
        }

        public string GetAsyncRefreshData(string tradeId, string time)
        {
            return _asyncRefreshDataTemplate
                .Replace("{{TRADE_ID}}", tradeId)
                .Replace("{{MILLISECONDS}}", time);
        }
    }
}
