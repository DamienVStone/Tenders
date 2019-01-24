using System.Collections.Generic;
using System.Threading.Tasks;
using Tenders.API.Parameters;

namespace Tenders.API.Services.Interfaces
{
    public interface IEntrySaverService
    {
        Task<int> SaveRootEntriesWithoutChildren(string PathId, IEnumerable<FTPEntryParam> Entries);
        Task<bool> SaveFTPEntriesTree(string PathId, FTPEntriesTreeParam RootEntry);
    }
}
