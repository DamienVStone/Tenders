using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Elastic
{
    public abstract class BaseElasticRepo<T> : IAPIRepository<T> where T : ModelBase
    {
        protected readonly ElasticClient Client;

        protected BaseElasticRepo(IElasticDbContext dbContext)
        {
            Client = dbContext.Client;
        }

        protected abstract T MapFields(FieldValues fields);

        public T GetOne(string id)
        {
            var resp = Client.Get<T>(Guid.Parse(id));
            return MapFields(resp.Fields);
        }

        public string Create(T item)
        {
            var res = Client.IndexDocument(item);
            return res.IsValid ? item.Id.ToString() : "";
        }

        public bool Delete(string id)
        {
            return Client.Delete<T>(Guid.Parse(id)).IsValid;
        }

        public IEnumerable<T> Get(int Skip, int Take, bool IsActive = true)
        {
            return Client.Search<T>(s => s
                .From(Skip)
                .Take(Take)
                .Query(q => q.
                    Term(t => t
                        .Field(f => f.IsActive)
                        .Value(IsActive)
                    )
                )
            ).Documents.AsEnumerable();
        }

        public bool Update(T item)
        {
            return Client.Update(new DocumentPath<T>(item.Id), u => u.Doc(item)).IsValid;
        }

        public bool ChangeActiveFlag(string Id, bool IsActive)
        {
            return Client.Update<T, object>(new DocumentPath<T>(Guid.Parse(Id)), u => u.Doc(new { IsActive })).IsValid;
        }

        public bool Exists(string Id, bool IsActive = true)
        {
            var res = Client.Count<T>(c => c
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(IsActive)
                            ), mu => mu
                            .Term(t => t
                                .Field(f => f.Id)
                                .Value(Guid.Parse(Id))
                            )
                        )
                    )
                )
            );

            return res.Count != 0;
        }

        public long CountAll(bool IsActive = true)
        {
            return Client.Count<T>(c => c
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.IsActive)
                        .Value(IsActive)
                    )
                )
            ).Count;
        }
    }
}
