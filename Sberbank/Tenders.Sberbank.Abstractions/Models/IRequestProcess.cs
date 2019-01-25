using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IRequestProcess : IProcess
    {
        int BidOrder { get; set; }
        PurchaseRequestResult Result { get; set; }
    }
}
