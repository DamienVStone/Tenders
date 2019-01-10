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
                Id = resp.Fields["id"].As<Guid>(),
                Path = resp.Fields["path"].As<string>(),
                Login = resp.Fields["login"].As<string>(),
                Password = resp.Fields["password"].As<string>(),
                IsActive = resp.Fields["isActive"].As<bool>(),
                CreatedDate = resp.Fields["createdDate"].As<DateTime>()
            };

            return path;
        }

        public void Update(FTPPath item)
        {
            _client.Update(new DocumentPath<FTPPath>(item.Id), u => u.Doc(item));
        }
    }
}
