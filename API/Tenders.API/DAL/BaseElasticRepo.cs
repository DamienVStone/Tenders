using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public abstract class BaseElasticRepo<T> : IAPIRepository<T> where T : ModelBase
    {
        protected readonly ElasticClient Client;

        protected BaseElasticRepo(IElasticDbContext dbContext)
        {
            Client = dbContext.Client;
        }

        protected abstract T MapFields(FieldValues fields);

        public T GetOne(Guid id)
        {
            var resp = Client.Get<T>(id);
            return MapFields(resp.Fields);
        }

        public void Create(T item)
        {
            Client.IndexDocument(item);
        }

        public void Delete(Guid id)
        {
            Client.Delete<T>(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Client.Search<T>(s => s.Query(q => q.MatchAll())).Documents.AsEnumerable();
        }

        public void Update(T item)
        {
            Client.Update(new DocumentPath<T>(item.Id), u => u.Doc(item));
        }
    }
}
