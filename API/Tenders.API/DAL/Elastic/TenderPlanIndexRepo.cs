using Nest;
using System;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.Models;

namespace Tenders.API.DAL.Elastic
{
    public class TenderPlanIndexRepo : BaseElasticRepo<TenderPlanIndex>
    {
        public TenderPlanIndexRepo(IElasticDbContext dbContext) : base(dbContext) { }

        protected override TenderPlanIndex MapFields(FieldValues fil)
        {
            return new TenderPlanIndex()
            {
                Id = fil["id"].As<Guid>().ToString(),
                CreatedDate = fil["createdDate"].As<DateTime>(),
                IsActive = fil["isActive"].As<bool>(),
                FTPFileId = fil["ftpFileId"].As<Guid>().ToString(),
                TenderPlanId = fil["tednerPlanId"].As<string>(),
                RevisionId = fil["revisionId"].As<long>(),
                IsOutdated = fil["isOutdated"].As<bool>()
            };
        }
    }
}
