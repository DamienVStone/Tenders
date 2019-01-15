using Nest;
using System;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Elastic
{
    public class TenderPlanIndexElasticRepo : BaseElasticRepo<TenderPlanIndex>, ITenderPlanIndexRepo
    {
        public TenderPlanIndexElasticRepo(IElasticDbContext dbContext) : base(dbContext) { }

        public TenderPlanIndex GetByExternalId(string Id)
        {
            return Client.Search<TenderPlanIndex>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(true)
                            ), mu => mu
                            .Term(t => t
                                .Field(f => f.TenderPlanId)
                                .Value(Id)
                            )
                        )
                    )
                )
            ).Documents.First();
        }

        public bool ExistsByExternalId(string Id)
        {
            return Client.Count<TenderPlanIndex>(c => c
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(true)
                            ), mu => mu
                            .Term(t => t
                                .Field(f => f.TenderPlanId)
                                .Value(Id)
                            )
                        )
                    )
                )
            ).Count != 0;
        }

        protected override TenderPlanIndex MapFields(FieldValues fields)
        {
            return new TenderPlanIndex()
            {
                Id = fields["id"].As<Guid>(),
                CreatedDate = fields["createdDate"].As<DateTime>(),
                IsActive = fields["isActive"].As<bool>(),
                FTPFileId = fields["ftpFileId"].As<Guid>(),
                TenderPlanId = fields["tenderPlanId"].As<string>(),
                RevisionId = fields["revisionId"].As<long>(),
                IsOutdated = fields["isOutdated"].As<bool>()
            };
        }
    }
}
