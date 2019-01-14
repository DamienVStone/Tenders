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
        public IActionResult Post([FromQuery]string pathId, [FromBody]ISet<FTPEntryParam> rootInputFiles)
        {
            if (rootInputFiles.Count == 0) return BadRequest("Нет файлов для добавления");
            if (string.IsNullOrWhiteSpace(pathId)) return BadRequest("Не указан идентификатор пути");
            if (!Guid.TryParse(pathId, out Guid g)) return BadRequest("Неверный идентификатор пути");
            if (!_pathRepo.Exists(pathId)) return BadRequest("Путь не найден");

            ISet<FTPEntry> pathFilesWithNoParents = new HashSet<FTPEntry>();
            // Постранично получаю вообще все файлы из директории
            var found = 0;
            var i = 0;
            do
            {
                var foundEntries = _entryRepo.GetByPath(i++ * 1000, 1000, pathId);
                pathFilesWithNoParents.UnionWith(foundEntries);
                found = foundEntries.Count();
            } while (found >= 1000);

            pathFilesWithNoParents.AsParallel().ForAll(p => p.State = StateFile.Pending);
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
                            lock (key)
                            {
                                dbFile.State = StateFile.Indexed;
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
                            Path = Guid.Parse(pathId),
                            IsDirectory = false, // в корне котолога нет директорий. директории только внутри зипников.
                            Parent = null
                        };

                        _entryRepo.Create(newFile);
                    }
                });

            //Удаляю все файлы которых уже нет на FTP сервере
            var forDelete = dbFiles.Values.Where(f => f.State == StateFile.Pending).Select(f => f.Id).ToArray();
            var delFailed = 0;
            foreach(var entry in forDelete)
            {
                if (!_entryRepo.Delete(entry.ToString())) delFailed++;
            }

            return Ok("Все файлы успешно добавлены."+(delFailed>0?$" Не удалось удалить файлов: {delFailed}":""));
        }

        /// <summary>
        /// Индексация деревьев файлов из ZIP архивов
        /// </summary>
        /// <param name="fileTree">Объект, хранящий дерево файлов и корень</param>
        /// <returns>200OK если индексация файлов прошла без ошибок</returns>
        [HttpPost("AddFileTree")]
        public IActionResult AddFileTree([FromBody] FileTreeParam fileTree)
        {
            if (!_pathRepo.Exists(fileTree.PathId.ToString())) return BadRequest("Путь не найден");

            var treeRoot = fileTree.TreeRoot;
            //Осторожно!
            //Корень дерева не может содержать родителя. Если у кроня дерева есть родитель то это, из-за дублируемых имен вложенных файлов, сломает всю концепцию индексатора и весь этот кусок надо будет переписать.
            if (!string.IsNullOrWhiteSpace(fileTree.TreeRoot.Parent.ToString())) return BadRequest("Корень дерева не может содержать родителя");

            //Осторожно!
            //Корень дерева не может быть директорией из-за дублирующих имен вложеных папок, это может сломать всю концепцию индексатора
            if (treeRoot.IsDirectory || !treeRoot.Name.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) return BadRequest("Корень дерева может быть только ZIP архивом");
            
            FTPEntry treeRootFromDb;
            if (!_entryRepo.ExistsByName(fileTree.TreeRoot.Name))
            {
                treeRootFromDb = new FTPEntry()
                {
                    Name = treeRoot.Name,
                    Size = treeRoot.Size,
                    Modified = treeRoot.DateModified,
                    Parent = Guid.Parse(treeRoot.Parent),
                    IsDirectory = treeRoot.IsDirectory,
                    State = StateFile.New,
                    Path = Guid.Parse(fileTree.PathId)
                };

                _entryRepo.Create(treeRootFromDb);
            }
            else
            {
                treeRootFromDb = _entryRepo.GetByName(fileTree.TreeRoot.Name);
                
                if (treeRootFromDb.Size != treeRoot.Size || !treeRootFromDb.Modified.Equals(treeRoot.DateModified))
                {
                    treeRootFromDb.Size = treeRoot.Size;
                    treeRootFromDb.Modified = treeRoot.DateModified;
                    treeRootFromDb.State = StateFile.Modified;

                    _entryRepo.Update(treeRootFromDb);
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

        private IEnumerable<FTPEntry> getAllChildren(FTPEntry root)
        {
            var tree = _entryRepo.GetByParentId(root.Id.ToString()).SelectMany(getAllChildren).ToHashSet();
            tree.Add(root);
            return tree;
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
