using System.Collections.Generic;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;

namespace Tenders.API.Services.Interfaces
{
    public interface ITreeLookerService
    {
        void UpdateFiles(string PathId, IEnumerable<FTPEntry> DbFiles, IEnumerable<FTPEntryParam> InputFiles, string DbParentId, string InputParentId);
    }
}
