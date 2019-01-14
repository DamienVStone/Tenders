using Nest;
using System;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Services.Interfaces;

namespace Tenders.API.DAL
{
    public class ElasticDBContext : IElasticDbContext
    {
        private static readonly object key = new object();
        public ElasticDBContext(IAPIConfigService config)
        {
            var settings = new ConnectionSettings(new Uri(config.DbConnectionString))
                            .DefaultMappingFor<FTPPath>(m => m.IndexName("ftpPath"))
                            .DefaultMappingFor<FTPEntry>(m => m.IndexName("ftpEntry"))
                            .DefaultMappingFor<TenderPlanIndex>(m => m.IndexName("tenderPlanIndex"));

            Client = new ElasticClient(settings);
        }

        public ElasticClient Client { private set; get; }
    }
}
