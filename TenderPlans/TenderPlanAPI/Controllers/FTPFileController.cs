using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using TenderPlanAPI.Classes;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using TenderPlanAPI.Viewmodels;

namespace TenderPlanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPFileController : ControllerBase
    {
        /// <summary>
        /// Добавление и обновления файлов у которых нет родителей
        /// </summary>
        /// <param name="pathId">Id объекта папки в которой лежат файлы</param>
        /// <param name="file">Массив файлов для добавления</param>
        /// <returns>200OK или ошибку, если что-то пошло не так</returns>
        [HttpPost]
        public IActionResult Post([FromQuery]string pathId, [FromBody]List<FTPEntryParam> rootInputFiles)
        {
            if(rootInputFiles.Count == 0)
            {
                return BadRequest("Нет файлов для добавления");
            }

            var db = new DBConnectContext();

            var path = getPathOrNull(ObjectId.Parse(pathId));
            if (path == null)
            {
                return BadRequest("Путь не найден");
            }

            var pathFilesFilter = Builders<FTPEntry>.Filter.Eq("Path", path.Id);
            var noParentFilesFilter = Builders<FTPEntry>.Filter.Eq("Parent", ObjectId.Empty);

            var pathAndNoParentFilter = Builders<FTPEntry>.Filter.And(pathFilesFilter, noParentFilesFilter);

            var pathFilesWithNoParents = db.FTPEntry.Find(pathAndNoParentFilter).ToList();
            pathFilesWithNoParents.ForEach(f => f.State = StateFile.Pending);

            var dbFiles = pathFilesWithNoParents.ToDictionary(f => f.Name);
            var key = new object();
            rootInputFiles
                .AsParallel()
                .ForAll(f =>
                {
                    if (dbFiles.Keys.Contains(f.Name))
                    {
                        //Файл существует в базе
                        var dbFile = dbFiles[f.Name];

                        var updates = new List<UpdateDefinition<FTPEntry>>();
                        var isFileModified = false;

                        if (dbFile.Size != f.Size)
                        {
                            updates.Add(Builders<FTPEntry>.Update.Set("Size", f.Size));
                            isFileModified = true;
                        }
                        if (!dbFile.Modified.Equals(f.DateModified))
                        {
                            updates.Add(Builders<FTPEntry>.Update.Set("Modified", f.DateModified));
                            isFileModified = true;
                        }

                        var filter = Builders<FTPEntry>.Filter.Eq("_id", dbFile.Id);
                        if (isFileModified)
                        {
                            lock (key)
                            {
                                dbFiles[f.Name].State = StateFile.Modified;
                            }
                            updates.Add(Builders<FTPEntry>.Update.Set("State", StateFile.Modified));
                            new DBConnectContext().FTPEntry.UpdateOne(filter, Builders<FTPEntry>.Update.Combine(updates));
                        }
                        else
                        {
                            lock (key)
                            {
                                dbFiles[f.Name].State = StateFile.Indexed;
                            }
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
                            Path = path.Id,
                            IsDirectory = false, // в корне котолога нет директорий. директории только внутри зипников.
                            Parent = ObjectId.Empty
                        };

                        new DBConnectContext().FTPEntry.InsertOne(newFile);
                    }
                });

            //Удаляю все файлы которых уже нет на FTP сервере
            var forDelete = dbFiles.Values.Where(f => f.State == StateFile.Pending).Select(f=>Builders<FTPEntry>.Filter.Eq("_id", f.Id)).ToArray();
            if(forDelete.Length>0)
                db.FTPEntry.DeleteMany(Builders<FTPEntry>.Filter.Or(forDelete));

            return Ok("");
        }
        
        /// <summary>
        /// Индексация деревьев файлов из ZIP архивов
        /// </summary>
        /// <param name="fileTree">Объект, хранящий дерево файлов и корень</param>
        /// <returns>200OK если индексация файлов прошла без ошибок</returns>
        [HttpPost("AddFileTree")]
        public IActionResult AddFileTree([FromBody] FileTreeParam fileTree)
        {
            var path = getPathOrNull(fileTree.PathId);
            if (path == null) return BadRequest("Путь не найден");

            var treeRoot = fileTree.TreeRoot;
            //Осторожно!
            //Корень дерева не может содержать родителя. Если у кроня дерева есть родитель то это, из-за дублируемых имен вложенных файлов, сломает всю концепцию индексатора и весь этот кусок надо будет переписать.
            if (treeRoot.Parent != null && !treeRoot.Parent.Equals(ObjectId.Empty)) return BadRequest("Корень дерева не может содержать родителя");

            //Осторожно!
            //Корень дерева не может быть директорией т.к., из-за дублирующих имен вложеных папок, это может сломать всю концепцию индексатора
            if (treeRoot.IsDirectory || !treeRoot.Name.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) return BadRequest("Корень дерева может быть только ZIP архивом");

            var treeRootNameFilter = Builders<FTPEntry>.Filter.Eq("Name", fileTree.TreeRoot.Name);
            var treeRootNoParentFilter = Builders<FTPEntry>.Filter.Eq("Parent", ObjectId.Empty);
            var treeRootFilter = Builders<FTPEntry>.Filter.And(treeRootNameFilter, treeRootNoParentFilter);

            var treeRootFromDb = new DBConnectContext().FTPEntry.Find(treeRootFilter).FirstOrDefault();
            if(treeRootFromDb == null)
            {
                treeRootFromDb = new FTPEntry()
                {
                    Name = treeRoot.Name,
                    Size = treeRoot.Size,
                    Modified = treeRoot.DateModified,
                    Parent = treeRoot.Parent,
                    IsDirectory = treeRoot.IsDirectory,
                    State = StateFile.New,
                    Path = path.Id
                };

                new DBConnectContext().FTPEntry.InsertOne(treeRootFromDb);
            }
            else
            {
                var updates = new List<UpdateDefinition<FTPEntry>>();
                var isModified = false;
                if (treeRootFromDb.Size != treeRoot.Size)
                {
                    updates.Add(Builders<FTPEntry>.Update.Set("Size", treeRoot.Size));
                    isModified = true;
                }
                
                if(!treeRootFromDb.Modified.Equals(treeRoot.DateModified)){
                    updates.Add(Builders<FTPEntry>.Update.Set("Modified", treeRoot.DateModified));
                    isModified = true;
                }

                if (isModified)
                {
                    updates.Add(Builders<FTPEntry>.Update.Set("State", StateFile.Modified));
                    var updateQuery = Builders<FTPEntry>.Update.Combine(updates);
                    new DBConnectContext().FTPEntry.UpdateOne(treeRootFilter, updateQuery);
                    treeRootFromDb = new DBConnectContext().FTPEntry.Find(treeRootFilter).FirstOrDefault();
                }
                else
                {
                    treeRootFromDb.State = StateFile.Indexed;
                }
            }

            //Далее получаю все дерево из бд для которого данный файл являтся родителем.
            var treeFromDb = getAllChildren(treeRootFromDb);

            var treeLooker = new TreeLooker(path.Id, treeFromDb, fileTree.Files);
            treeLooker.UpdateFiles(treeRootFromDb.Id, fileTree.TreeRoot.Id);

            return Ok("");
        }

        /// <summary>
        /// Возвращает все вновь созданные или измененные файлы
        /// </summary>
        /// <returns>Список файлов</returns>
        [HttpGet]
        public IEnumerable<FTPEntry> Get()
        {
            var db = new DBConnectContext();
            var filter1 = Builders<FTPEntry>.Filter.Eq("State", StateFile.New);
            var filter2 = Builders<FTPEntry>.Filter.Eq("State", StateFile.Modified);
            var filterOr = Builders<FTPEntry>.Filter.Or(new List<FilterDefinition<FTPEntry>> { filter1, filter2 });
            var find = db.FTPEntry.Find(filterOr).ToList();
            return find;
        }

        /// <summary>
        /// Возвращает список файлов, которые находятся по пути с указанным Id.
        /// </summary>
        /// <param name="pathId">Идентификатор FTP пути</param>
        /// <returns></returns>
        [HttpGet("GetByPath")]
        public ActionResult<IEnumerable<FTPEntry>> GetByPath([FromQuery]string pathId)
        {
            if (string.IsNullOrWhiteSpace(pathId))
            {
                return BadRequest("Необходимо указать идентификатор пути.");
            }

            ObjectId id;
            if(!ObjectId.TryParse(pathId, out id))
            {
                return BadRequest("Неверный формат идентификатора пути");
            }

            var pathIdFilter = Builders<FTPEntry>.Filter.Eq("Path", id);
            var zipFilter = Builders<FTPEntry>.Filter.Regex("Name", @".*\.zip");
            var notZipFilter = Builders<FTPEntry>.Filter.Not(zipFilter);
            var filter = Builders<FTPEntry>.Filter.And(pathIdFilter, notZipFilter);
            return new DBConnectContext().FTPEntry.Find(filter).ToList();
        }

        /// <summary>
        /// Возвращает список файлов планов тендеров для индексатора с определенным статусом
        /// </summary>
        /// <param name="fileState">Статус возвращаемых файлов. По умолчанию возвращаются новые и обновленные файлы</param>
        /// <returns></returns>
        [HttpGet("GetTenderPlansToIndex")]
        public ActionResult<IEnumerable<TenderPlanFileToIndexViewmodel>> GetTenderPlansToIndexer([FromQuery]StateFile? fileState)
        {
            FilterDefinition<FTPEntry> fileStateFilter;
            if (fileState.HasValue)
            {
                fileStateFilter = Builders<FTPEntry>.Filter.Eq("State", fileState);
            }
            else
            {
                var newFilter = Builders<FTPEntry>.Filter.Eq("State", StateFile.New);
                var editFilter = Builders<FTPEntry>.Filter.Eq("State", StateFile.Modified);
                fileStateFilter = Builders<FTPEntry>.Filter.Or(newFilter, editFilter);
            }

            var tenderPlansFilter = Builders<FTPEntry>.Filter.Regex("Name", ".*tenderPlan.*");
            var zipFilter = Builders<FTPEntry>.Filter.Regex("Name", @".*\.zip");
            var notZipFilter = Builders<FTPEntry>.Filter.Not(zipFilter);
            var filter = Builders<FTPEntry>.Filter.And(tenderPlansFilter, fileStateFilter, notZipFilter);
            return new DBConnectContext().FTPEntry
                .Find(filter)
                .ToEnumerable()
                .Select(p => 
                    new TenderPlanFileToIndexViewmodel {
                        FTPFileId = p.Id,
                        Name = p.Name
                    })
                .ToList();
        }

        /// <summary>
        /// Возвращает список файлов, которые находятся по пути с указанным Id и указанным статусом.
        /// </summary>
        /// <param name="pathId">Идентификатор FTP пути</param>
        /// <param name="state">Статус файлов</param>
        /// <returns></returns>
        [HttpGet("GetByPathAndState")]
        public ActionResult<IEnumerable<FTPEntry>> GetByPathAndState([FromQuery] string pathId, [FromQuery]StateFile state)
        {
            if (string.IsNullOrWhiteSpace(pathId))
            {
                return BadRequest("Необходимо указать идентификатор пути");
            }

            ObjectId id;
            if(!ObjectId.TryParse(pathId, out id))
            {
                return BadRequest("Неверно указан идентификатор пути");
            }

            var pathFilter = Builders<FTPEntry>.Filter.Eq("Path", id);
            var stateFilter = Builders<FTPEntry>.Filter.Eq("State", state);
            var pathAndStateFilter = Builders<FTPEntry>.Filter.And(pathFilter, stateFilter);

            return new DBConnectContext().FTPEntry.Find(pathAndStateFilter).ToList();
        }

        private FTPPath getPathOrNull(ObjectId pathId)
        {
            var pathFilter = Builders<FTPPath>.Filter.Eq("_id", pathId);
            var path = new DBConnectContext().FTPPath.Find(pathFilter).FirstOrDefault();
            return path;
        }

        private List<FTPEntry> getAllChildren(FTPEntry root)
        {
            var filter = Builders<FTPEntry>.Filter.Eq("Parent", root.Id);
            var children = new DBConnectContext().FTPEntry.Find(filter).ToList().SelectMany(getAllChildren).ToList();
            children.Add(root);
            return children;
        }
    }
}
