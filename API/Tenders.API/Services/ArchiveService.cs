using System;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Enums;
using Tenders.API.Models;
using Tenders.API.Services.Interfaces;

namespace Tenders.API.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly IFTPEntryRepo _repo;
        private readonly object key = new object();
        public ArchiveService(IFTPEntryRepo repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public bool ArchiveMonitoringFailed(string Id)
        {
            if (!_repo.Exists(Id)) throw new ArgumentException("Элемент не существует");
            var entry = _repo.GetOne(Id);
            entry.State = StateFile.Corrupted;
            return _repo.Update(entry);
        }

        public FTPEntry GetNextArchiveForMonitoring()
        {
            lock (key)
            {
                var entry = _repo.GetRandomNewOrModifiedArchive();
                if (entry == null) return null;
                entry.State = StateFile.Indexed;
                _repo.Update(entry);
                return entry;
            }
        }
    }
}
