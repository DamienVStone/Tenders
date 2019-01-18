using Tenders.API.Models;

namespace Tenders.API.Services.Interfaces
{
    public interface IArchiveService
    {
        FTPEntry GetNextArchiveForMonitoring();
        FTPEntry ArchiveMonitoringFailed(string Id);
    }
}
