using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Tenders.API.DAL.Interfaces;
using Tenders.API.DAL.Mongo.Interfaces;
using Tenders.API.Enums;
using Tenders.API.Models;
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

        public bool ExistsArchive(string Id)
        {
            return Entities.CountDocuments(f => f.IsActive && f.IsArchive && f.Id == Id)!=0;
        }

        public bool ExistsByNameAndPathAndIsDirectoryAndIsArchive(string Name, string PathId, bool IsDirectory, bool HasParents = false, bool IsArchive = true)
        {
            CheckId(PathId);
            return Entities
                .CountDocuments(f => 
                    f.IsActive && 
                    f.Path == PathId && 
                    f.Name == Name && 
                    f.IsDirectory == IsDirectory && 
                    ((HasParents && f.Parent!=null) || (!HasParents && f.Parent == null)) &&
                    f.IsArchive == IsArchive) != 0;
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
            CheckId(PathId);
            return Entities
                .Find(f => f.IsActive && f.Path == PathId && States.Any(i => i == f.State) && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public FTPEntry GetByNameAndPathAndIsDirectoryAndIsArchive(string Name, string PathId, bool IsDirectory, bool HasParents = false, bool IsArchive = true)
        {
            CheckId(PathId);

            return Entities
                .Find(f => 
                    f.IsActive && 
                    f.Path == PathId && 
                    f.Name == Name && 
                    f.IsDirectory==IsDirectory && 
                    ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)) &&
                    f.IsArchive == IsArchive    
                )
                .Limit(1)
                .First();
        }

        public IEnumerable<FTPEntry> GetByParentId(string ParentId)
        {
            CheckId(ParentId);
            return Entities
                .Find(f => f.IsActive && f.Parent == ParentId)
                .ToEnumerable();
        }

        public IEnumerable<FTPEntry> GetByPath(int Skip, int Take, string PathId, bool HasParents = false)
        {
            CheckId(PathId);
            return Entities
                .Find(f => f.IsActive && f.Path == PathId && ((HasParents && f.Parent != null) || (!HasParents && f.Parent == null)))
                .Skip(Skip)
                .Limit(Take)
                .ToEnumerable();
        }

        public FTPEntry GetRandomNewOrModifiedArchive()
        {
            return Entities
                .Find(f => f.IsActive && (f.State == StateFile.New || f.State == StateFile.Modified))
                .Limit(1)
                .FirstOrDefault();
        }
    }
}
