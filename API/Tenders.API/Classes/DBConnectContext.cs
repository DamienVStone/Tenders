using MongoDB.Driver;
using TenderPlanAPI.Models;
using Tenders.API.Services;

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

        public DBConnectContext(IAPIConfigService config)
        {
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
        }

        public IMongoCollection<Customer> Customers => new TransientDBContext(config).Customers;

        public IMongoCollection<TenderPlan> TenderPlans => new TransientDBContext(config).TenderPlans;

        public IMongoCollection<TenderPlanPosition> TenderPlanPositions => new TransientDBContext(config).TenderPlanPositions;

        public IMongoCollection<TenderPlanIndex> TenderPlansIndex => new TransientDBContext(config).TenderPlansIndex;

        public IMongoCollection<FTPPath> FTPPath => new TransientDBContext(config).FTPPath;

        public IMongoCollection<FTPEntry> FTPEntry => new TransientDBContext(config).FTPEntry;

        private class TransientDBContext
        {
            private readonly IAPIConfigService config;

            public TransientDBContext(IAPIConfigService config)
            {
                this.config = config ?? throw new System.ArgumentNullException(nameof(config));
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
                    return new MongoClient(config.DbConnectionString);
                }
            }


            private IMongoCollection<T> _connectionTenderPlan<T>(string name)
            {
                var db = client.GetDatabase(config.DbName);
                return db.GetCollection<T>(name);
            }

            private IMongoCollection<T> _connectionFTPMonitor<T>(string name)
            {
                var db = client.GetDatabase(config.DbName);
                return db.GetCollection<T>(name);
            }
        }

    }


}
