using System.Collections.Generic;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPEntryRepo : IAPIRepository<FTPEntry>
    {
        IEnumerable<FTPEntry> GetFilesFromPath(int Skip, int Take, string PathId, bool HasParents = false);

    }
}
