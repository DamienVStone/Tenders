namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IPurchaseOfferMyNewPriceContainer
    {
        string pricesign { get; set; }
        string purchaseCode { get; set; }
        string purchaseID { get; set; }
        string purchaseName { get; set; }
        string reqid { get; set; }
        string requestNo { get; set; }
        string supplierName { get; set; }
    }
}