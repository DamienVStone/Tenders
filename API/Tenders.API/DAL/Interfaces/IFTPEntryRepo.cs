using System;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPEntryRepo : IAPIRepository<FTPEntry>
    {
        FTPEntry GetFilesFromPath(Guid PathId, bool HasParents = false);

    }
}
