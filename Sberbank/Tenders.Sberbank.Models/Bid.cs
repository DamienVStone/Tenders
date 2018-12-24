using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class Bid : IBid
    {
        public string reqID { get; set; }
        public string xmlData { get; set; }
        public string Hash { get; set; }
    }
}