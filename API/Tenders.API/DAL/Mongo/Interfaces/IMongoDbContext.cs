using MongoDB.Driver;
using Tenders.API.Models;

namespace Tenders.API.DAL.Mongo.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<FTPPath> FTPPaths { get; }
        IMongoCollection<FTPEntry> FTPEntries { get; }
        IMongoCollection<TenderPlanIndex> TenderPlanIndices { get; }
    }
}
