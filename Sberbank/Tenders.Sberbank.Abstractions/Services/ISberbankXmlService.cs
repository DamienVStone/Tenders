using System.Collections.Generic;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Models.Requesting;

namespace Tenders.Sberbank.Abstractions.Services
{
    /// <summary>
    /// Сервис обработки xml данных от сбербанка
    /// </summary>
    public interface ISberbankXmlService
    {
        IEnumerable<ISearchResultEntry> GetSearchResult(string xmlFilterResult);
        ITradePlace GetTradePlace(string s);
        //void LoadXML(string xmlId, string xmlContainerId, HtmlDocument lotDocument);
        IPurchaseRequest GetPurchaseRequest(string s);
        string GetXml(IPurchaseRequest purchaseRequest);
        string GetXml(IElasticRequest purchaseRequest);
    }
}
