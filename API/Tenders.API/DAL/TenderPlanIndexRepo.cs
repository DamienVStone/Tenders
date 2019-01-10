using Nest;
using System;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class TenderPlanIndexRepo : BaseElasticRepo<TenderPlanIndex>
    {
        public TenderPlanIndexRepo(IElasticDbContext dbContext) : base(dbContext) {}

        protected override TenderPlanIndex MapFields(FieldValues fil)
        {
            return new TenderPlanIndex()
            {
                Id = fil["id"].As<Guid>(),
                CreatedDate = fil["createdDate"].As<DateTime>(),
                IsActive = fil["isActive"].As<bool>(),
                FTPFileId = fil["ftpFileId"].As<Guid>(),
                TenderPlanId = fil["tednerPlanId"].As<string>(),
                RevisionId = fil["revisionId"].As<long>(),
                IsOutdated = fil["isOutdated"].As<bool>()
            };
        }
    }
}
