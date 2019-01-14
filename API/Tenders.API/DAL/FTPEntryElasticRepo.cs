using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL
{
    public class FTPEntryElasticRepo : BaseElasticRepo<FTPEntry>, IFTPEntryRepo
    {

        public FTPEntryElasticRepo(IElasticDbContext DbContext):base(DbContext) {}

        public IEnumerable<FTPEntry> GetFilesFromPath(int Skip, int Take, string PathId, bool HasParents = false)
        {
            var pathId = Guid.Parse(PathId);
            return Client.Search<FTPEntry>(s => s
                .From(Skip)
                .Take(Take)
                .Query(q => q
                    .Bool(b => 
                        mustHaveParents(
                            b.Must(mu => mu
                                .Term(t => t
                                    .Field(f => f.Path)
                                    .Value(pathId)

                                ), mu => mu
                                .Term(t => t
                                    .Field(f => f.IsActive)
                                    .Value(true)
                                )
                            ),
                            HasParents
                        )
                    )
                )
            ).Documents.AsEnumerable();
        }

        private BoolQueryDescriptor<FTPEntry> mustHaveParents(BoolQueryDescriptor<FTPEntry> descriptor, bool HaveParents)
        {
            if (HaveParents)
            {
                return descriptor.Must(m => m
                            .Exists(e => e
                                .Field(f => f.Parent)
                            )
                        );
            }
            else
            {
                return descriptor.MustNot(mn => mn
                            .Exists(e => e
                                .Field(f => f.Parent)
                            )
                        );
            }
        }

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
