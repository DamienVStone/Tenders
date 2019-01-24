using System;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface ILot
    {
        Uri Url { get; set; }
        object Text { get; set; }
        object RegNumber { get; set; }
    }
}
