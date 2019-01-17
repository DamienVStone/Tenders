using MongoDB.Driver;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.API.Models;
using Tenders.API.Services.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.DAL.Mongo
{
    public class MongoDbContext: IMongoDbContext
    {
        private IAPIConfigService _config;
        private ILoggerService _logger;

        public MongoDbContext(IAPIConfigService config, ILoggerService logger)
        {
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public IMongoCollection<FTPPath> FTPPaths => _connection<FTPPath>(nameof(FTPPath));

        public IMongoCollection<FTPEntry> FTPEntries => _connection<FTPEntry>(nameof(FTPEntry));

        public IMongoCollection<TenderPlanIndex> TenderPlanIndices => _connection<TenderPlanIndex>(nameof(TenderPlanIndex));

        private MongoClient client
        {
            get
            {
                try
                {
                    _logger.Log("Creating mongo client with connection string : " + _config.DbConnectionString);
                    var c = new MongoClient(_config.DbConnectionString);
                    return c;
                }
                catch (System.Exception e)
                {
                    _logger.Log(e.ToString());
                    throw;
                }
            }
        }


        private IMongoCollection<T> _connection<T>(string name)
        {
            try
            {
                _logger.Log($"Getting database {_config.DbName}");
                var db = client.GetDatabase(_config.DbName);
                _logger.Log($"Getting collection {name}");
                var collection = db.GetCollection<T>(name);
                return collection;
            }
            catch (System.Exception e)
            {
                _logger.Log(e.ToString());
                throw;
            }
        }

        

    }
}
