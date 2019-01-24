using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models.Requesting
{
    public interface IElasticRequest
    {
        void Init(ISearchParameters request);
    }
}