using Nest;

namespace Tenders.API.DAL.Elastic.Interfaces
{
    public interface IElasticDbContext
    {
        ElasticClient Client { get; }
    }
}
