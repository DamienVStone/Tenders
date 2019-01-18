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
        FTPEntry GetByNameAndPathAndIsDirectoryAndIsArchive(string Name, string PathId, bool IsDirectory, bool HasParents = false, bool IsArchive = true);
        bool ExistsByNameAndPathAndIsDirectoryAndIsArchive(string Name, string PathId, bool IsDirectory, bool HasParents = false, bool IsArchive = true);
        IEnumerable<FTPEntry> GetByParentId(string ParentId);
        FTPEntry GetRandomNewOrModifiedArchive();
        bool ExistsArchive(string Id);
    }
}
