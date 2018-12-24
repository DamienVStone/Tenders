namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IPurchaseofferPurchoffer
    {
        object MyOfferMark { get; set; }
        string offerKind { get; set; }
        byte offerPlace { get; set; }
        decimal offerPrice { get; set; }
        string offerTime { get; set; }
        byte reqno { get; set; }
    }
}