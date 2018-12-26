using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Services
{
    /// <summary>
    /// Сервис обработки сериализованных данных от сбербанка
    /// </summary>
    public interface ISberbankDeserializationService
    {
        ISearchResult GetSearchResult(string xmlFilterResult);
        ITradePlace GetTradePlace(string s);
    }
}
