using System.Collections.Generic;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPEntryRepo : IAPIRepository<FTPEntry>
    {
        IEnumerable<FTPEntry> GetByPath(int Skip, int Take, string PathId, bool HasParents = false);
        IEnumerable<FTPEntry> GetByFileState(int Skip, int Take, bool HasParents = false, params StateFile[] States);
        IEnumerable<FTPEntry> GetByFileStateAndPath(int Skip, int Take, string PathId, bool HasParents = false, params StateFile[] States);
    }
}
