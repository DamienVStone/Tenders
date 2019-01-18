using Tenders.API.Models;

namespace Tenders.API.Services.Interfaces
{
    public interface IArchiveService
    {
        FTPEntry GetNextArchiveForMonitoring();
        bool ArchiveMonitoringFailed(string Id);
    }
}
