using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Enums;
using Tenders.API.Models;
using Tenders.API.Parameters;
using Tenders.API.Services.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Services
{
    public class EntrySaverService : IEntrySaverService
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;
        private readonly IIdProvider _idProvider;
        private readonly ILoggerService _logger;

        public EntrySaverService(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo, IIdProvider idProvider, ILoggerService logger)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
            _idProvider = idProvider ?? throw new ArgumentNullException(nameof(idProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SaveFTPEntriesTree(string PathId, string RootId, FTPEntriesTreeParam rootEntry)
        {
            var entriesToAdd = new HashSet<FTPEntry>();
            var sw = new Stopwatch();
            sw.Start();
            //var entry = 
            //    _entryRepo.ExistsByNameAndPathAndIsDirectoryAndIsArchive(rootEntry.Name, PathId, rootEntry.IsDirectory, rootEntry.IsArchive)? 
            //    _entryRepo.GetByNameAndPathAndIsDirectoryAndIsArchive(rootEntry.Name, PathId, rootEntry.IsDirectory, rootEntry.IsArchive) : null;
            //FTPEntry entry = null;
            //var res = _putEntry(entry, rootEntry, null, PathId, entriesToAdd);
            //await _logger.Log($"Сохранил корень: {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            sw.Restart();
            _saveTree(RootId, rootEntry.Children, PathId, entriesToAdd);
            await _logger.Log($"Обошел дерево {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            sw.Restart();
            if (entriesToAdd.Count > 0) await _entryRepo.BulkInsert(entriesToAdd);
            await _logger.Log($"Записал новые файлы в базу {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            sw.Stop();
            return true;
        }

        public async Task<int> SaveRootEntriesWithoutChildren(string PathId, IEnumerable<FTPEntryParam> Entries)
        {
            //ISet<FTPEntry> pathFilesWithNoParents = new HashSet<FTPEntry>();
            // Постранично получаю вообще все файлы из директории
            //var found = 0;
            //var i = 0;
            //do
            //{
            //    var foundEntries = _entryRepo.GetByPath(i++ * 1000, 1000, PathId);
            //    pathFilesWithNoParents.UnionWith(foundEntries);
            //    found = foundEntries.Count();
            //} while (found >= 1000);


            var entriesToCreate = new HashSet<FTPEntry>();

            //pathFilesWithNoParents.AsParallel().ForAll(p => p.State = StateFile.Pending);
            //var dbFiles = pathFilesWithNoParents.ToDictionary(f => $"{f.Name}_{f.IsDirectory}_{f.IsArchive}");
            //var updateKey = new object();
            var createKey = new object();
            Entries
                .AsParallel()
                .ForAll(f =>
                {
                    //var k = $"{f.Name}_{f.IsDirectory}_{f.IsArchive}";
                    //if (dbFiles.Keys.Contains(k))
                    //{
                    //    //Файл существует в базе
                    //    FTPEntry dbFile;
                    //    lock(updateKey)dbFile = dbFiles[k];
                    //    if (dbFile.Size != f.Size || !dbFile.Modified.Equals(f.DateModified))
                    //    {
                    //        lock (updateKey)
                    //        {
                    //            dbFile.Name = f.Name;
                    //            dbFile.Size = f.Size;
                    //            dbFile.State = StateFile.Modified;
                    //        }
                    //        _entryRepo.Update(dbFile);
                    //    }
                    //    else
                    //    {
                    //        lock (updateKey) dbFile.State = StateFile.Indexed;
                    //        //не обновляю в базе сознтельно!
                    //    }
                    //}
                    //else
                    {
                        //Файл не существует в базе
                        string id;
                        lock (createKey) id = ObjectId.GenerateNewId().ToString();
                        var newFile = new FTPEntry
                        {
                            Id = id,
                            Name = f.Name,
                            Modified = f.DateModified,
                            Size = f.Size,
                            Path = PathId,
                            IsDirectory = false, // в корне каталога нет директорий. директории только внутри зипников.
                            Parent = null,
                            IsArchive = f.IsArchive
                        };

                        lock (createKey) entriesToCreate.Add(newFile);
                    }
                });

            if (entriesToCreate.Count > 0)
                //_entryRepo.CreateMany(entriesToCreate);
                await _entryRepo.BulkInsert(entriesToCreate);

            //Удаляю все файлы которых уже нет на FTP сервере
            //var forDelete = dbFiles.Values.Where(f => f.State == StateFile.Pending).Select(f => f.Id).ToArray();
            //var delFailed = 0;
            //foreach (var entry in forDelete)
            //{
            //    if (!_entryRepo.Delete(entry.ToString())) delFailed++;
            //}

            //return delFailed;
            return 0;
        }

        private void _saveTree(string parentId, IEnumerable<FTPEntriesTreeParam> inputChildren, string pathId, ISet<FTPEntry> entriesToAdd)
        {
            if (inputChildren == null || inputChildren.Count() == 0) return;
            var sw = new Stopwatch();
            sw.Start();

            //Dictionary<string, FTPEntry> dbChildren = _entryRepo.GetByParentId(parentId).ToDictionary(dc => $"{dc.Name}_{dc.IsDirectory}_{dc.IsArchive}");
            //_logger.Log($"Получил все элементы по идентификатору родителя {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            //sw.Restart();
            var i = 0;
            foreach (var c in inputChildren)
            {
                var key = $"{c.Name}_{c.IsDirectory}_{c.IsArchive}";
                //var res = _putEntry(dbChildren.ContainsKey(key) ? dbChildren[key] : null, c, parentId, pathId, entriesToAdd);
                var res = _putEntry(null, c, parentId, pathId, entriesToAdd);
                if (c.Children == null || c.Children.Count() == 0) continue;
                _saveTree(res, c.Children, pathId, entriesToAdd);
                i++;
            }
            _logger.Log($"Цикл по детям {i} за {sw.Elapsed}");
            sw.Stop();
        }

        private string _putEntry(FTPEntry o, FTPEntriesTreeParam n, string parent, string pathId, ISet<FTPEntry> entriesToAdd)
        {
            var sw = new Stopwatch();
            sw.Start();
            string res;
            //if (o == null)
            {
                res = _idProvider.GenerateId();
                var entry = new FTPEntry()
                {
                    Id = res,
                    Name = n.Name,
                    IsDirectory = n.IsDirectory,
                    IsArchive = n.IsArchive,
                    Size = n.Size,
                    Modified = n.Modified,
                    Path = pathId,
                    Parent = parent,
                    State = StateFile.New
                };
                entriesToAdd.Add(entry);
            }
            //else
            //{
            //    var modified = n.Size != o.Size || n.Modified != o.Modified;
            //    if (modified)
            //    {
            //        o.Size = n.Size;
            //        o.Modified = n.Modified;
            //        o.State = StateFile.Modified;

            //        _entryRepo.Update(o);
            //    }
            //    res = o.Id;
            //}
            _logger.Log($"Добавил элемент {sw.ElapsedMilliseconds}");
            sw.Stop();
            return res ?? throw new InvalidOperationException($"Ошибка при сохранении пути {n.Name}. Идентификатор сохраненного объекта оказался null");
        }

    }
}
