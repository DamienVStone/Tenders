using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IRequestProcess : IProcess
    {
        string LotId { get; set; }
        PurchaseRequestResult Result { get; set; }
    }
}
