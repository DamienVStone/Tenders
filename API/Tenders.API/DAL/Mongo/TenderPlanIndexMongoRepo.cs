using MongoDB.Driver;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.API.Models;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.DAL.Mongo
{
    public class TenderPlanIndexMongoRepo : BaseMongoRepo<TenderPlanIndex>, ITenderPlanIndexRepo
    {
        private IMongoDbContext _db;
        public TenderPlanIndexMongoRepo(IMongoDbContext dbContext, IIdProvider idProvider, ILoggerService logger) : base(idProvider, logger)
        {
            _db = dbContext;
        }

        protected override IMongoCollection<TenderPlanIndex> Entities => _db.TenderPlanIndices;

        public bool ExistsByExternalId(string Id)
        {
            return Entities.CountDocuments(f => f.IsActive && f.TenderPlanId == Id)>0;
        }

        public TenderPlanIndex GetByExternalId(string Id)
        {
            return Entities.Find(f => f.IsActive && f.TenderPlanId == Id).Limit(1).First();
        }
    }
}
