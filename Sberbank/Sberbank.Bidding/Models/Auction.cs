using System;

namespace Sberbank.Bidding.Models
{
    public class Auction
    {
        public string Code { get; set; }
        public DateTime StartTime { get; set; }
        public string Info { get; set; }
        public LotState State { get; set; }
        public decimal MinCost { get; set; }
        public int Workers { get; set; }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
