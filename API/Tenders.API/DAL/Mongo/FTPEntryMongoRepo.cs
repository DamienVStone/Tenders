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

        public bool ExistsByNameAndPathAndIsDirectory(string Name, string PathId, bool IsDirectory, bool HasParents = false)
        {
            checkId(PathId);
            return Entities.CountDocuments(f => f.IsActive && f.Path == PathId && f.Name == Name && f.IsDirectory == IsDirectory && ((HasParents && f.Parent!=null) || (!HasParents && f.Parent == null))) != 0;
        }

        public IEnumerable<FTPEntry> GetByFileState(int Skip, int Take, bool HasParents = false, params StateFile[] States)
        {
            return Entities
                .Find(f => f.IsActive && States.Any(i => i == f.State) && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public IEnumerable<FTPEntry> GetByFileStateAndPath(int Skip, int Take, string PathId, bool HasParents = false, params StateFile[] States)
        {
            checkId(PathId);
            return Entities
                .Find(f => f.IsActive && f.Path == PathId && States.Any(i => i == f.State) && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public FTPEntry GetByNameAndPathAndIsDirectory(string Name, string PathId, bool IsDirectory, bool HasParents = false)
        {
            checkId(PathId);

            return Entities
                .Find(f => f.IsActive && f.Path == PathId && f.Name == Name && f.IsDirectory==IsDirectory && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
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
                .Find(f => f.IsActive && f.Path == PathId && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        
    }
}
