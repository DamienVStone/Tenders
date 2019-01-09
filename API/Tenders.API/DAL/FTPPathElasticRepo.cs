using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class FTPPathElasticRepo : IAPIRepository<FTPPath>
    {
        private ElasticClient _client;
        public FTPPathElasticRepo(IElasticDbContext DbContext)
        {
            _client = DbContext.Client;
        }

        public void Create(FTPPath item)
        {
            _client.IndexDocument(item);
        }

        public void Delete(Guid id)
        {
            _client.Delete<FTPPath>(id);
        }

        public IEnumerable<FTPPath> GetAll()
        {
            return _client.Search<FTPPath>(s => s.Query(q => q.MatchAll())).Documents.AsEnumerable();
        }

        public FTPPath GetOne(Guid id)
        {
            var resp = _client.Get<FTPPath>(id);

            var path = new FTPPath {
                Id = resp.Fields.
            };

            return ;
        }

        public void Update(FTPPath item)
        {
            
        }
    }
}
