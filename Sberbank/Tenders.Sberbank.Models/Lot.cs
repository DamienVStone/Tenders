using System;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class Lot : ILot
    {
        public Uri Url { get; set; }
        public object Text { get; set; }
        public object RegNumber { get; set; }
    }
}