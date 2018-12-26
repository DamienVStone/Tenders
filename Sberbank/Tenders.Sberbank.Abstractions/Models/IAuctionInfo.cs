namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IAuctionInfo
    {
        string RegNumber { get; set; }
        int Workers { get; set; }
        decimal Bid { get; set; }
    }
}
