namespace Tenders.Sberbank.Abstractions.Models
{
    public interface ITradePlace
    {
        string ReqID { get; set; }
        string ReqNo { get; set; }
        string PriceSign { get; set; }
        string PurchId { get; set; }
        string PurchCode { get; set; }
        string PurchName { get; set; }
        string SuppName { get; set; }
    }
}
