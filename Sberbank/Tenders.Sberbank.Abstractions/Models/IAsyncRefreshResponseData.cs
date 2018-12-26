namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IAsyncRefreshResponseData
    {
        string __type { get; set; }
        string curentTime { get; set; }
        bool HideTradePlace { get; set; }
        string shortTime { get; set; }
        string xmlData { get; set; }
        string xmlError { get; set; }
        string xmlResult { get; set; }
    }
}