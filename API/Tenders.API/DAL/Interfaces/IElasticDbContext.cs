using Nest;

namespace Tenders.API.DAL.Interfaces
{
    public interface IElasticDbContext
    {
        ElasticClient Client { get; }
    }
}
