using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface ITenderPlanIndexRepo : IAPIRepository<TenderPlanIndex>
    {
        TenderPlanIndex GetByExternalId(string Id);
        bool ExistsByExternalId(string Id);
    }
}
