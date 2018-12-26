using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class AuctionInfo : IAuctionInfo
    {
        public int Workers { get; set; }
        public string RegNumber { get; set; }
        public decimal Bid { get; set; }
    }
}
