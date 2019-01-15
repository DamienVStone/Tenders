using MongoDB.Driver;
using System.Collections.Generic;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo.Interfaces;

namespace Tenders.API.DAL.Mongo
{
    public class FTPPathMongoRepo : BaseMongoRepo<FTPPath>, IFTPPathRepo
    {
        private IMongoDbContext _db;

        public FTPPathMongoRepo(IMongoDbContext db, IIdProvider idProvider): base(idProvider)
        {
            _db = db ?? throw new System.ArgumentNullException(nameof(db));
        }

        protected override IMongoCollection<FTPPath> Entities => _db.FTPPaths;

        public FTPPath GetSinglePathByName(string PathName, bool IsActive = true)
        {
            return Entities.Find(f => f.IsActive == IsActive && f.Path == PathName).Limit(1).FirstOrDefault();
        }

        public bool PathExistsByName(string PathName, bool IsActive = true)
        {
            return Entities.CountDocuments(f => f.IsActive == IsActive && f.Path == PathName) != 0;
        }
        
    }
}
