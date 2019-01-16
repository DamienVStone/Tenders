using MongoDB.Driver;
using System;
using System.Collections.Generic;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.DAL.Mongo
{
    public class FTPPathMongoRepo : BaseMongoRepo<FTPPath>, IFTPPathRepo
    {
        private IMongoDbContext _db;

        public FTPPathMongoRepo(IMongoDbContext db, IIdProvider idProvider, ILoggerService logger): base(idProvider, logger)
        {
            _db = db ?? throw new System.ArgumentNullException(nameof(db));
        }

        protected override IMongoCollection<FTPPath> Entities => _db.FTPPaths;

        public FTPPath GetOldestIndexedPath(int Timeout)
        {
            var filter = Builders<FTPPath>.Filter.Lte("LastTimeIndexed", DateTimeOffset.Now.AddHours(-Timeout));
            var update = Builders<FTPPath>.Update.Set("LastTimeIndexed", DateTimeOffset.Now);
            return Entities.FindOneAndUpdate(filter, update);
        }

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
