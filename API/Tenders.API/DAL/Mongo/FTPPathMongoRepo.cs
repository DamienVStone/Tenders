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

        protected override UpdateDefinition<FTPPath> createUpdateDefinition(FTPPath db, FTPPath inp)
        {
            var updates = new HashSet<UpdateDefinition<FTPPath>>();
            if (db.IsActive != inp.IsActive) updates.Add(Builders<FTPPath>.Update.Set(f => f.IsActive, inp.IsActive));
            if (db.LastTimeIndexed != inp.LastTimeIndexed) updates.Add(Builders<FTPPath>.Update.Set(f => f.LastTimeIndexed, inp.LastTimeIndexed));
            if (db.Id != inp.Id) updates.Add(Builders<FTPPath>.Update.Set(f => f.Id, inp.Id));
            if (db.Path != inp.Path) updates.Add(Builders<FTPPath>.Update.Set(f => f.Path, inp.Path));
            if (db.Login != inp.Login) updates.Add(Builders<FTPPath>.Update.Set(f => f.Login, inp.Login));
            if (db.Password != db.Password) updates.Add(Builders<FTPPath>.Update.Set(f => f.Password, inp.Password));
            
            return Builders<FTPPath>.Update.Combine(updates);
        }
    }
}
