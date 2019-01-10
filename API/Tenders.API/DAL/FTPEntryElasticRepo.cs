using System;
using Nest;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class FTPEntryElasticRepo : BaseElasticRepo<FTPEntry>
    {

        public FTPEntryElasticRepo(IElasticDbContext DbContext):base(DbContext) {}

        protected override FTPEntry MapFields(FieldValues fields)
        {
            return new FTPEntry() {
                Id = fields["id"].As<Guid>(),
                CreatedDate = fields["createdDate"].As<DateTime>(),
                IsActive = fields["isActive"].As<bool>(),
                Name = fields["name"].As<string>(),
                Size = fields["size"].As<long>(),
                IsDirectory = fields["isDirectory"].As<bool>(),
                Path = fields["path"].As<Guid>(),
                Parent = fields["parent"]?.As<Guid>()
            };
        }
    }
}
