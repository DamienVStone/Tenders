using System.Collections.Generic;
using Tenders.API.Enums;
using Tenders.API.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPEntryRepo : IAPIRepository<FTPEntry>
    {
        IEnumerable<FTPEntry> GetByPath(int Skip, int Take, string PathId, bool HasParents = false);
        IEnumerable<FTPEntry> GetByFileState(int Skip, int Take, bool HasParents = false, params StateFile[] States);
        IEnumerable<FTPEntry> GetByFileStateAndPath(int Skip, int Take, string PathId, bool HasParents = false, params StateFile[] States);
        FTPEntry GetByNameAndPathAndIsDirectory(string Name, string PathId, bool IsDirectory, bool HasParents = false);
        bool ExistsByNameAndPathAndIsDirectory(string Name, string PathId, bool IsDirectory, bool HasParents = false);
        IEnumerable<FTPEntry> GetByParentId(string ParentId);
    }
}
