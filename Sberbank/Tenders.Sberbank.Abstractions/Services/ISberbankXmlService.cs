using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Abstractions.Models.PurchaseRequest;

namespace Tenders.Sberbank.Abstractions.Services
{
    /// <summary>
    /// Сервис обработки xml данных от сбербанка
    /// </summary>
    public interface ISberbankXmlService
    {
        ISearchResult GetSearchResult(string xmlFilterResult);
        ITradePlace GetTradePlace(string s);
        //void LoadXML(string xmlId, string xmlContainerId, HtmlDocument lotDocument);
        IPurchaseRequest GetPurchaseRequest(string s);
        string GetXmlFromPurchaseRequest(IPurchaseRequest purchaseRequest);
    }
}
