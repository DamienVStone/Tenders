using System;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Models;
using Tenders.API.Services.Interfaces;

namespace Tenders.API.Services
{
    public class PathService : IPathService
    {
        private readonly object key = new object();
        private readonly IAPIConfigService config;
        private readonly IFTPPathRepo ftpPathRepo;

        public PathService(IAPIConfigService Config, IFTPPathRepo FtpPathRepo)
        {
            config = Config ?? throw new ArgumentNullException(nameof(Config));
            ftpPathRepo = FtpPathRepo ?? throw new ArgumentNullException(nameof(FtpPathRepo));
        }

        public FTPPath GetNotIndexedPath()
        {
            lock (key)
            {
                return ftpPathRepo.GetOldestIndexedPath(config.FTPIndexingTimeout);
            }
        }
    }
}
