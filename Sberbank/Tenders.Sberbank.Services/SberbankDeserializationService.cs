﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;
using Tenders.Core.Abstractions.Services;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;

namespace Tenders.Sberbank.Services
{
    public class SberbankDeserializationService : ISberbankDeserializationService
    {
        private readonly ILoggerService loggerService;

        public SberbankDeserializationService(
            ILoggerService loggerService
            )
        {
            this.loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
        }

        public ISearchResult GetSearchResult(string s)
        {
            var result = (data)new XmlSerializer(typeof(data)).Deserialize(new StringReader(s));
            return new SearchResult()
            {
                Entries = result.Entries
            };
        }

        public ITradePlace GetTradePlace(string s)
        {
            var resp = JsonConvert.DeserializeObject<AsyncRefreshResponse<D>>(s);
            try
            {
                if (!string.IsNullOrEmpty(resp.d.xmlError))
                {
                    var errorInfo = (Errors)new XmlSerializer(typeof(Errors)).Deserialize(new StringReader(resp.d.xmlError));
                    loggerService.Log(errorInfo.Error.ErrorMessage);
                }

                if (!string.IsNullOrEmpty(resp.d.xmlResult))
                {
                    loggerService.Log(resp.d.xmlResult);
                }

                var aucInfo = (purchaseoffer)
                    new XmlSerializer(typeof(purchaseoffer))
                    .Deserialize(new StringReader(resp.d.xmlData));

                return new TradePlace
                {
                    PriceSign = aucInfo.MyNewPriceContainer.pricesign,
                    PurchCode = aucInfo.purchcode,
                    PurchId = aucInfo.purchid,
                    PurchName = aucInfo.purchname,
                    ReqNo = aucInfo.reqno,
                    SuppName = aucInfo.suppname,
                    ReqID = aucInfo.MyNewPriceContainer.reqid
                };
            }
            catch (Exception e)
            {
                var errorInfo = (Errors)new XmlSerializer(typeof(Errors)).Deserialize(new StringReader(resp.d.xmlData));
                loggerService.Log(errorInfo.Error.ErrorMessage);
                throw new Exception(errorInfo.Error.ErrorMessage, e);
            }
        }
    }
}
