using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPEntryRepo : IAPIRepository<FTPEntry>
    {
        FTPEntry GetFilesFromPath(string PathId, bool HasParents = false);

    }
}
