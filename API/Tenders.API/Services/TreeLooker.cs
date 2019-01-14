using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Services.Interfaces;

namespace TenderPlanAPI.Services
{
    public class TreeLookerService : ITreeLookerService
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;

        public TreeLookerService(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
        }

        public void UpdateFiles(string PathId, ISet<FTPEntry> DbFiles, ISet<FTPEntryParam> InputFiles, string DbParentId, string InputParentId)
        {
            var pathId = Guid.Parse(PathId);
            var dbParentId = Guid.Parse(DbParentId);
            var inputParentId = Guid.Parse(InputParentId);

            var key = new object();
            var dbFolderContents = DbFiles.Where(f => f.Parent.Equals(dbParentId)).ToDictionary(f => f.Name);
            var inputFolderContents = InputFiles.Where(f => f.Parent.Equals(inputParentId));

            inputFolderContents
                .AsParallel()
                .ForAll(f =>
                {
                    FTPEntry dbFile;

                    if (dbFolderContents.Keys.Contains(f.Name))
                    {
                        //Файл существует в базе
                        dbFile = dbFolderContents[f.Name];
                        if (dbFile.Size != f.Size || !dbFile.Modified.Equals(f.DateModified))
                        {
                            lock (key)
                            {
                                dbFile.Modified = f.DateModified;
                                dbFile.Size = f.Size;
                                dbFile.State = StateFile.Modified;
                            }
                            _entryRepo.Update(dbFile);
                        }
                        else
                        {
                            lock (key)
                            {
                                dbFile.State = StateFile.Indexed;
                            }
                        }
                    }
                    else
                    {
                        //Файл не существует в базе
                        dbFile = new FTPEntry
                        {
                            Name = f.Name,
                            Modified = f.DateModified,
                            Size = f.Size,
                            Path = pathId,
                            Parent = dbParentId
                        };

                        dbFile.Id = Guid.Parse(_entryRepo.Create(dbFile));
                    }
                    // Вызываю ту же функцию для данного файла, чтобы проиндексироваьт всех детей
                    UpdateFiles(PathId, DbFiles, InputFiles, dbFile.Id.ToString(), f.Id.ToString());

                });

            dbFolderContents.Values.Where(f => f.State == StateFile.Pending).Select(f => f.Id.ToString()).AsParallel().ForAll(e => _entryRepo.Delete(e));
        }
    }
}
