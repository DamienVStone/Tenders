using Nest;
using System;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class FTPPathElasticRepo : BaseElasticRepo<FTPPath>
    {
        public FTPPathElasticRepo(IElasticDbContext DbContext) : base(DbContext) { }

        protected override FTPPath MapFields(FieldValues Fields)
        {
            var path = new FTPPath
            {
                Id = Fields["id"].As<Guid>(),
                Path = Fields["path"].As<string>(),
                Login = Fields["login"].As<string>(),
                Password = Fields["password"].As<string>(),
                IsActive = Fields["isActive"].As<bool>(),
                CreatedDate = Fields["createdDate"].As<DateTime>()
            };

            return path;
        }


    }
}
