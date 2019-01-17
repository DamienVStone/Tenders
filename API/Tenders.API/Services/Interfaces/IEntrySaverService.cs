using System.Collections.Generic;
using TenderPlanAPI.Parameters;
using Tenders.API.Parameters;

namespace Tenders.API.Services.Interfaces
{
    public interface IEntrySaverService
    {
        int SaveRootEntriesWithoutChildren(string PathId, IEnumerable<FTPEntryParam> Entries);
        void SaveFTPEntriesTree(string PathId, FTPEntriesTreeParam RootEntry);
    }
}
