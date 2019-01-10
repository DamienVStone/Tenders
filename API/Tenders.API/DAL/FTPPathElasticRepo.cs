using System;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class FTPPathElasticRepo : BaseElasticRepo<FTPPath>
    {
        public FTPPathElasticRepo(IElasticDbContext DbContext) : base(DbContext) { }

        public override FTPPath GetOne(Guid id)
        {
            var resp = Client.Get<FTPPath>(id);

            var path = new FTPPath
            {
                Id = resp.Fields["id"].As<Guid>(),
                Path = resp.Fields["path"].As<string>(),
                Login = resp.Fields["login"].As<string>(),
                Password = resp.Fields["password"].As<string>(),
                IsActive = resp.Fields["isActive"].As<bool>(),
                CreatedDate = resp.Fields["createdDate"].As<DateTime>()
            };

            return path;
        }


    }
}
