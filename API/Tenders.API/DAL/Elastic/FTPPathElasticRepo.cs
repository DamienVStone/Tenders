using Nest;
using System;
using System.Linq;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Elastic.Interfaces;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Elastic
{
    public class FTPPathElasticRepo : BaseElasticRepo<FTPPath>, IFTPPathRepo
    {
        public FTPPathElasticRepo(IElasticDbContext DbContext) : base(DbContext) { }

        public FTPPath GetSinglePathByName(string PathName, bool IsActive = true)
        {
            return Client.Search<FTPPath>(s => s
                .Size(1)
                .Query(q => q
                    .Bool(b =>
                        b.Must(mu => mu
                            .Term(t => t
                                .Field(f => f.Path)
                                .Value(PathName)
                            ), mu => mu
                            .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(IsActive)
                            )
                        )
                    )
                )
            ).Documents.AsEnumerable().FirstOrDefault();
        }

        public bool PathExistsByName(string PathName, bool IsActive = true)
        {
            var res = Client.Count<FTPPath>(c => c
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field(f => f.IsActive)
                                .Value(IsActive)
                            ), mu => mu
                            .Term(t => t
                                .Field(f => f.Path)
                                .Value(PathName)
                            )
                        )
                    )
                )
            );

            return res.Count != 0;
        }

        protected override FTPPath MapFields(FieldValues Fields)
        {
            var path = new FTPPath
            {
                Id = Fields["id"].As<Guid>().ToString(),
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
