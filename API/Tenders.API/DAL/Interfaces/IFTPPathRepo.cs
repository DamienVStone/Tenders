using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPPathRepo : IAPIRepository<FTPPath>
    {
        FTPPath GetSinglePathByName(string PathName, bool IsActive = true);
    }
}
