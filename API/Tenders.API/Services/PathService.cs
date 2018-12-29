using MongoDB.Driver;
using System;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Models;
using Tenders.API.Services;

namespace TenderPlanAPI.Services
{
    public interface IPathService
    {
        FTPPath GetNotIndexedPath();
    }

    // Это синглтон. Смотри конфигурацию в файле Startup
    public class PathService : IPathService
    {
        private readonly object key = new object();
        private readonly IAPIConfigService config;
        private readonly IDBConnectContext dbContext;

        public PathService(IAPIConfigService config, IDBConnectContext dbContext) : base()
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public FTPPath GetNotIndexedPath()
        {
            lock (key)
            {
                var timeout = config.FTPIndexingTimeout;
                var filter = Builders<FTPPath>.Filter.Lte("LastTimeIndexed", DateTimeOffset.Now.AddHours(-timeout));
                var update = Builders<FTPPath>.Update.Set("LastTimeIndexed", DateTimeOffset.Now);
                return dbContext.FTPPath.FindOneAndUpdate(filter, update);
            }
        }
    }
}
