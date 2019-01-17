using System.Collections.Generic;
using Tenders.API.Models;
using Tenders.API.Parameters;

namespace Tenders.API.Services.Interfaces
{
    public interface ITreeLookerService
    {
        void UpdateFiles(string PathId, IEnumerable<FTPEntry> DbFiles, IEnumerable<FTPEntryParam> InputFiles, string DbParentId, string InputParentId);
    }
}
