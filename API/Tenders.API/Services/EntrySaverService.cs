using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Parameters;
using Tenders.API.Services.Interfaces;

namespace TenderPlanAPI.Services
{
    public class EntrySaverService : IEntrySaverService
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;

        public EntrySaverService(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
        }

        public void SaveFTPEntriesTree(string PathId, FTPEntriesTreeParam rootEntry)
        {
            var entry = _entryRepo.ExistsByNameAndPathAndIsDirectory(rootEntry.Name, PathId, rootEntry.IsDirectory) ? _entryRepo.GetByNameAndPathAndIsDirectory(rootEntry.Name, pathId, rootEntry.IsDirectory) : null;
            var res = _putEntry(entry, rootEntry, null, PathId);
            _saveTree(res, rootEntry.Children, PathId);
        }

        public int SaveRootEntriesWithoutChildren(string PathId, IEnumerable<FTPEntryParam> Entries)
        {
            ISet<FTPEntry> pathFilesWithNoParents = new HashSet<FTPEntry>();
            // Постранично получаю вообще все файлы из директории
            var found = 0;
            var i = 0;
            do
            {
                var foundEntries = _entryRepo.GetByPath(i++ * 1000, 1000, PathId);
                pathFilesWithNoParents.UnionWith(foundEntries);
                found = foundEntries.Count();
            } while (found >= 1000);

            pathFilesWithNoParents.AsParallel().ForAll(p => p.State = StateFile.Pending);
            var dbFiles = pathFilesWithNoParents.ToDictionary(f => f.Name);
            var key = new object();
            Entries
                .AsParallel()
                .ForAll(f =>
                {
                    if (dbFiles.Keys.Contains(f.Name))
                    {
                        //Файл существует в базе
                        FTPEntry dbFile;
                        lock(key)dbFile = dbFiles[f.Name];
                        if (dbFile.Size != f.Size || !dbFile.Modified.Equals(f.DateModified))
                        {
                            lock (key)
                            {
                                dbFile.Name = f.Name;
                                dbFile.Size = f.Size;
                                dbFile.State = StateFile.Modified;
                            }
                            _entryRepo.Update(dbFile);
                        }
                        else
                        {
                            lock (key) dbFile.State = StateFile.Indexed;
                            
                        }
                    }
                    else
                    {
                        //Файл не существует в базе
                        var newFile = new FTPEntry
                        {
                            Name = f.Name,
                            Modified = f.DateModified,
                            Size = f.Size,
                            Path = PathId,
                            IsDirectory = false, // в корне котолога нет директорий. директории только внутри зипников.
                            Parent = null
                        };

                        _entryRepo.Create(newFile);
                    }
                });

            //Удаляю все файлы которых уже нет на FTP сервере
            var forDelete = dbFiles.Values.Where(f => f.State == StateFile.Pending).Select(f => f.Id).ToArray();
            var delFailed = 0;
            foreach (var entry in forDelete)
            {
                if (!_entryRepo.Delete(entry.ToString())) delFailed++;
            }

            return delFailed;
        }

        private void _saveTree(string parentId, IEnumerable<FTPEntriesTreeParam> inputChildren, string pathId)
        {
            if (inputChildren == null) return;
            Dictionary<string, FTPEntry> dbChildren = _entryRepo.GetByParentId(parentId).ToDictionary(dc => $"{dc.Name}_{dc.IsDirectory}");
            inputChildren.AsParallel().ForAll(c =>
            {
                var key = $"{c.Name}_{c.IsDirectory}";
                var res = _putEntry(dbChildren.ContainsKey(key) ? dbChildren[key] : null, c, parentId, pathId);
                _saveTree(res, c.Children, pathId);
            });
        }

        private string _putEntry(FTPEntry o, FTPEntriesTreeParam n, string parent, string pathId)
        {
            string res;
            if (o == null)
            {
                var entry = new FTPEntry()
                {
                    Name = n.Name,
                    IsDirectory = n.IsDirectory,
                    Size = n.Size,
                    Modified = n.Modified,
                    Path = pathId,
                    Parent = parent,
                    State = StateFile.New
                };
                res = _entryRepo.Create(entry);
            }
            else
            {
                var modified = n.Size != o.Size || n.Modified != o.Modified;
                if (modified)
                {
                    o.Size = n.Size;
                    o.Modified = n.Modified;
                    o.State = StateFile.Modified;

                    _entryRepo.Update(o);
                }
                res = o.Id;
            }


            return res ?? throw new InvalidOperationException($"Ошибка при сохранении пути {n.Name}. Идентификатор сохраненного объекта оказался null");
        }
        
    }
}
