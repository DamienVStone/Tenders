using System.Collections.Generic;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;

namespace Tenders.API.Services.Interfaces
{
    public interface ITreeLookerService
    {
        void UpdateFiles(string PathId, ISet<FTPEntry> DbFiles, ISet<FTPEntryParam> InputFiles, string DbParentId, string InputParentId);
    }
}
