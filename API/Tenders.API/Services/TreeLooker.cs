using System;
using System.Collections.Generic;
using System.Linq;
using Tenders.API.Enums;
using Tenders.API.Models;
using Tenders.API.Parameters;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Services.Interfaces;

namespace Tenders.API.Services
{
    public class TreeLookerService : ITreeLookerService
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;
        private readonly IIdProvider _idProvider;

        public TreeLookerService(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo, IIdProvider idProvider)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
            _idProvider = idProvider ?? throw new ArgumentNullException(nameof(idProvider));
        }

        public void UpdateFiles(string PathId, IEnumerable<FTPEntry> DbFiles, IEnumerable<FTPEntryParam> InputFiles, string DbParentId, string InputParentId)
        {
            var pathId = Guid.Parse(PathId);

            if (!_idProvider.IsIdValid(PathId)) throw new ArgumentException($"Неверный формат {nameof(PathId)}: {PathId}");
            if (!_idProvider.IsIdValid(DbParentId)) throw new ArgumentNullException($"Неверный формат {nameof(DbParentId)}: {DbParentId}");
            if (!_idProvider.IsIdValid(InputParentId)) throw new ArgumentException($"Неверный формат {nameof(InputParentId)}: {InputParentId}");

            

            var key = new object();
            var dbFolderContents = DbFiles.Where(f => f.Parent == DbParentId).ToDictionary(f => f.Name);
            var inputFolderContents = InputFiles.Where(f => f.Parent == InputParentId);

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
                            Path = PathId,
                            Parent = DbParentId
                        };

                        dbFile.Id = _entryRepo.Create(dbFile);
                    }
                    // Вызываю ту же функцию для данного файла, чтобы проиндексироваьт всех детей
                    UpdateFiles(PathId, DbFiles, InputFiles, dbFile.Id.ToString(), f.Id.ToString());

                });

            dbFolderContents.Values.Where(f => f.State == StateFile.Pending).Select(f => f.Id.ToString()).AsParallel().ForAll(e => _entryRepo.Delete(e));
        }
    }
}
