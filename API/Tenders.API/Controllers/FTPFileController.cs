using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TenderPlanAPI.Classes;
using TenderPlanAPI.Enums;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using TenderPlanAPI.Viewmodels;
using Tenders.API.DAL.Interfaces;

namespace TenderPlanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPFileController : ControllerBase
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;

        public FTPFileController(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
        }



        /// <summary>
        /// Добавление и обновления файлов у которых нет родителей
        /// </summary>
        /// <param name="pathId">Id объекта папки в которой лежат файлы</param>
        /// <param name="file">Массив файлов для добавления</param>
        /// <returns>200OK или ошибку, если что-то пошло не так</returns>
        [HttpPost]
        public IActionResult Post([FromQuery]string pathId, [FromBody]List<FTPEntryParam> rootInputFiles)
        {
            if (rootInputFiles.Count == 0)
            {
                return BadRequest("Нет файлов для добавления");
            }

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
                            db.FTPEntry.UpdateOne(filter, Builders<FTPEntry>.Update.Combine(updates));
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

                        db.FTPEntry.InsertOne(newFile);
                    }
                });

            //Удаляю все файлы которых уже нет на FTP сервере
            var forDelete = dbFiles.Values.Where(f => f.State == StateFile.Pending).Select(f => Builders<FTPEntry>.Filter.Eq("_id", f.Id)).ToArray();
            if (forDelete.Length > 0)
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

            var treeRootFromDb = db.FTPEntry.Find(treeRootFilter).FirstOrDefault();
            if (treeRootFromDb == null)
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

                db.FTPEntry.InsertOne(treeRootFromDb);
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

                if (!treeRootFromDb.Modified.Equals(treeRoot.DateModified))
                {
                    updates.Add(Builders<FTPEntry>.Update.Set("Modified", treeRoot.DateModified));
                    isModified = true;
                }

                if (isModified)
                {
                    updates.Add(Builders<FTPEntry>.Update.Set("State", StateFile.Modified));
                    var updateQuery = Builders<FTPEntry>.Update.Combine(updates);
                    db.FTPEntry.UpdateOne(treeRootFilter, updateQuery);
                    treeRootFromDb = db.FTPEntry.Find(treeRootFilter).FirstOrDefault();
                }
                else
                {
                    treeRootFromDb.State = StateFile.Indexed;
                }
            }

            //Далее получаю все дерево из бд для которого данный файл являтся родителем.
            var treeFromDb = getAllChildren(treeRootFromDb);

            var treeLooker = new TreeLooker(path.Id, treeFromDb, fileTree.Files, db);
            treeLooker.UpdateFiles(treeRootFromDb.Id, fileTree.TreeRoot.Id);

            return Ok("");
        }

        /// <summary>
        /// Возвращает все вновь созданные или измененные файлы
        /// </summary>
        /// <returns>Список файлов</returns>
        [HttpGet]
        public ListResponse<FTPEntry> Get([FromQuery]FilterOptions options)
        {
            return new ListResponse<FTPEntry>() {
                Count = _entryRepo.CountAll(),
                Data = _entryRepo.Get(options.Skip, options.Take).ToArray()
            };
        }

        /// <summary>
        /// Возвращает список файлов, которые находятся по пути с указанным Id.
        /// </summary>
        /// <param name="pathId">Идентификатор FTP пути</param>
        /// <returns></returns>
        [HttpGet("GetByPath")]
        public ActionResult<ListResponse<FTPEntry>> GetByPath([FromQuery]string pathId, [FromQuery]FilterOptions options)
        {
            if (string.IsNullOrWhiteSpace(pathId))
            {
                return BadRequest("Необходимо указать идентификатор пути.");
            }

            Guid id;
            if (!Guid.TryParse(pathId, out id))
            {
                return BadRequest("Неверный формат идентификатора пути");
            }

            return new ListResponse<FTPEntry>()
            {
                Count = _entryRepo.CountAll(),
                Data = _entryRepo.GetByPath(options.Skip, options.Take, pathId).ToArray()
            };
        }

        /// <summary>
        /// Возвращает список файлов планов тендеров для индексатора с определенным статусом
        /// </summary>
        /// <param name="state">Статус возвращаемых файлов. По умолчанию возвращаются новые и обновленные файлы</param>
        /// <returns></returns>
        [HttpGet("GetTenderPlansToIndex")]
        public ActionResult<ListResponse<TenderPlanFileToIndexViewmodel>> GetTenderPlansToIndexer([FromQuery]StateFile? state, [FromQuery]FilterOptions options)
        {
            var states = state.HasValue ? new StateFile[] { state.Value } : new StateFile[] { StateFile.New, StateFile.Modified };
            var data = _entryRepo.GetByFileState(options.Skip, options.Take, true, states);
            return new ListResponse<TenderPlanFileToIndexViewmodel>() {
                Count = _entryRepo.CountAll(),
                Data = data.Select(e => new TenderPlanFileToIndexViewmodel() { FTPFileId = e.Id.ToString(), Name = e.Name }).ToArray()
            };
        }

        /// <summary>
        /// Возвращает список файлов, которые находятся по пути с указанным Id и указанным статусом.
        /// </summary>
        /// <param name="pathId">Идентификатор FTP пути</param>
        /// <param name="state">Статус файлов</param>
        /// <returns></returns>
        [HttpGet("GetByPathAndState")]
        public ActionResult<ListResponse<FTPEntry>> GetByPathAndState([FromQuery] string pathId, [FromQuery]StateFile? state, [FromQuery]FilterOptions options)
        {
            if (string.IsNullOrWhiteSpace(pathId))
            {
                return BadRequest("Необходимо указать идентификатор пути");
            }

            Guid id;
            if (!Guid.TryParse(pathId, out id))
            {
                return BadRequest("Неверно указан идентификатор пути");
            }

            var states = state.HasValue ? new StateFile[] { state.Value } : new StateFile[] { StateFile.New, StateFile.Modified };
            return new ListResponse<FTPEntry>()
            {
                Count = _entryRepo.CountAll(),
                Data = _entryRepo.GetByFileStateAndPath(options.Skip, options.Take, pathId, true, states).ToArray()
            };
        }

    }
}
