using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IPurchaseOfferErrors<T> : IXmlModel<IPurchaseOfferErrors<IPurchaseOfferErrorsError>>
        where T : IPurchaseOfferErrorsError
    {
        T Error { get; set; }
    }
}