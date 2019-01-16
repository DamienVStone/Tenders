using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.DAL.Mongo
{
    public class FTPEntryMongoRepo : BaseMongoRepo<FTPEntry>, IFTPEntryRepo
    {
        private IMongoDbContext _dbContext;

        public FTPEntryMongoRepo(IMongoDbContext dbContext, IIdProvider idProvider, ILoggerService logger) : base(idProvider, logger)
        {
            _dbContext = dbContext;
        }

        protected override IMongoCollection<FTPEntry> Entities => _dbContext.FTPEntries;

        public bool ExistsByName(string Name, bool HasParents = false)
        {
            return Entities.CountDocuments(f => f.IsActive && f.Name == Name && ((HasParents && !string.IsNullOrWhiteSpace(f.Parent)) || (!HasParents && string.IsNullOrWhiteSpace(f.Parent)))) != 0;
        }

        public IEnumerable<FTPEntry> GetByFileState(int Skip, int Take, bool HasParents = false, params StateFile[] States)
        {
            return Entities
                .Find(f => f.IsActive && States.Any(i => i == f.State) && ((HasParents && !string.IsNullOrWhiteSpace(f.Parent)) || (!HasParents && string.IsNullOrWhiteSpace(f.Parent))))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public IEnumerable<FTPEntry> GetByFileStateAndPath(int Skip, int Take, string PathId, bool HasParents = false, params StateFile[] States)
        {
            checkId(PathId);
            return Entities
                .Find(f => f.IsActive && f.Path == PathId && States.Any(i => i == f.State) && ((HasParents && !string.IsNullOrWhiteSpace(f.Parent)) || (!HasParents && string.IsNullOrWhiteSpace(f.Parent))))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public FTPEntry GetByName(string Name, bool HasParents = false)
        {
            return Entities
                .Find(f => f.IsActive && f.Name == Name && ((HasParents && !string.IsNullOrWhiteSpace(f.Parent)) || (!HasParents && string.IsNullOrWhiteSpace(f.Parent))))
                .Limit(1)
                .First();
        }

        public IEnumerable<FTPEntry> GetByParentId(string ParentId)
        {
            checkId(ParentId);
            return Entities
                .Find(f => f.IsActive && f.Parent == ParentId)
                .ToEnumerable();
        }

        public IEnumerable<FTPEntry> GetByPath(int Skip, int Take, string PathId, bool HasParents = false)
        {
            checkId(PathId);
            return Entities
                .Find(f => f.IsActive && f.Path == PathId && ((HasParents && !string.IsNullOrWhiteSpace(f.Parent)) || (!HasParents && string.IsNullOrWhiteSpace(f.Parent))))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }
    }
}
