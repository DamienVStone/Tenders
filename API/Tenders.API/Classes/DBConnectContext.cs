using MongoDB.Driver;
using TenderPlanAPI.Models;
using Tenders.API.Services;
using Tenders.Core.Abstractions.Services;

namespace TenderPlanAPI.Controllers
{
    public interface IDBConnectContext
    {
        IMongoCollection<Customer> Customers { get; }
        IMongoCollection<TenderPlan> TenderPlans { get; }
        IMongoCollection<TenderPlanPosition> TenderPlanPositions { get; }
        IMongoCollection<TenderPlanIndex> TenderPlansIndex { get; }
        IMongoCollection<FTPPath> FTPPath { get; }
        IMongoCollection<FTPEntry> FTPEntry { get; }
    }

    public class DBConnectContext : IDBConnectContext
    {
        private readonly IAPIConfigService config;
        private readonly ILoggerService logger;

        public DBConnectContext(
            IAPIConfigService config,
            ILoggerService logger)
        {
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public IMongoCollection<Customer> Customers => new TransientDBContext(config, logger).Customers;

        public IMongoCollection<TenderPlan> TenderPlans => new TransientDBContext(config, logger).TenderPlans;

        public IMongoCollection<TenderPlanPosition> TenderPlanPositions => new TransientDBContext(config, logger).TenderPlanPositions;

        public IMongoCollection<TenderPlanIndex> TenderPlansIndex => new TransientDBContext(config, logger).TenderPlansIndex;

        public IMongoCollection<FTPPath> FTPPath => new TransientDBContext(config, logger).FTPPath;

        public IMongoCollection<FTPEntry> FTPEntry => new TransientDBContext(config, logger).FTPEntry;

        private class TransientDBContext
        {
            private readonly IAPIConfigService config;
            private readonly ILoggerService logger;

            public TransientDBContext(
                IAPIConfigService config,
                ILoggerService logger
                )
            {
                this.config = config ?? throw new System.ArgumentNullException(nameof(config));
                this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            }

            public IMongoCollection<Customer> Customers => _connectionTenderPlan<Customer>("customers");
            public IMongoCollection<TenderPlan> TenderPlans => _connectionTenderPlan<TenderPlan>("tenderPlans");
            public IMongoCollection<TenderPlanPosition> TenderPlanPositions => _connectionTenderPlan<TenderPlanPosition>("tenderPlanPosition");

            public IMongoCollection<TenderPlanIndex> TenderPlansIndex
            {
                get
                {
                    var tpInd = _connectionTenderPlan<TenderPlanIndex>("tenderPlansIndex");
                    var crtInd = Builders<TenderPlanIndex>.IndexKeys.Ascending(tpi => tpi.TenderPlanId);
                    var crtIndOpt = new CreateIndexOptions<TenderPlanIndex> { Unique = true };
                    var crtIndModel = new CreateIndexModel<TenderPlanIndex>(crtInd, crtIndOpt);
                    tpInd.Indexes.CreateOne(crtIndModel);
                    return tpInd;
                }
            }

            public IMongoCollection<FTPPath> FTPPath => _connectionFTPMonitor<FTPPath>("ftpPath");

            public IMongoCollection<FTPEntry> FTPEntry => _connectionFTPMonitor<FTPEntry>("ftpFile");

            private MongoClient client
            {
                get
                {
                    try
                    {
                        logger.Log("Creating mongo client with connection string : " + config.DbConnectionString);
                        var c = new MongoClient(config.DbConnectionString);
                        return c;
                    }
                    catch (System.Exception e)
                    {
                        logger.Log(e.ToString());
                        throw;
                    }
                }
            }


            private IMongoCollection<T> _connectionTenderPlan<T>(string name)
            {
                try
                {
                    logger.Log($"Getting database {config.DbName}");
                    var db = client.GetDatabase(config.DbName);
                    logger.Log($"Getting collection {name}");
                    var collection = db.GetCollection<T>(name);
                    return collection;
                }
                catch (System.Exception e)
                {
                    logger.Log(e.ToString());
                    throw;
                }
            }

            private IMongoCollection<T> _connectionFTPMonitor<T>(string name)
            {
                var db = client.GetDatabase(config.DbName);
                return db.GetCollection<T>(name);
            }
        }

    }


}
