using Nest;
using System;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.Services.Interfaces;

namespace Tenders.API.DAL.Elastic
{
    public class ElasticDBContext : IElasticDbContext
    {
        private static readonly object key = new object();
        public ElasticDBContext(IAPIConfigService config)
        {
            var settings = new ConnectionSettings(new Uri(config.DbConnectionString))
                            .DefaultMappingFor<FTPPath>(m => m.IndexName("ftp-path"))
                            .DefaultMappingFor<FTPEntry>(m => m.IndexName("ftp-entry"))
                            .DefaultMappingFor<TenderPlanIndex>(m => m.IndexName("tender-plan-index"));

            Client = new ElasticClient(settings);
        }

        public ElasticClient Client { private set; get; }
    }
}
