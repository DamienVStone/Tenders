using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Enums;
using Tenders.API.Models;
using Tenders.API.Parameters;
using Tenders.API.Services.Interfaces;
using Tenders.API.Viewmodels;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPFileController : ControllerBase
    {
        private readonly IFTPEntryRepo _entryRepo;
        private readonly IFTPPathRepo _pathRepo;
        private readonly IEntrySaverService _treeSaverService;
        private readonly IIdProvider _idProvider;
        private readonly ILoggerService _logger;

        public FTPFileController(IFTPEntryRepo entryRepo, IFTPPathRepo pathRepo, IEntrySaverService treeLookerService, IIdProvider idProvider, ILoggerService logger)
        {
            _entryRepo = entryRepo ?? throw new ArgumentNullException(nameof(entryRepo));
            _pathRepo = pathRepo ?? throw new ArgumentNullException(nameof(pathRepo));
            _treeSaverService = treeLookerService ?? throw new ArgumentNullException(nameof(treeLookerService));
            _idProvider = idProvider ?? throw new ArgumentNullException(nameof(idProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Добавление и обновления файлов у которых нет родителей
        /// </summary>
        /// <param name="pathId">Id объекта папки в которой лежат файлы</param>
        /// <param name="file">Массив файлов для добавления</param>
        /// <returns>200OK или ошибку, если что-то пошло не так</returns>
        [HttpPost]
        public IActionResult Post([FromQuery]string pathId, [FromBody]IEnumerable<FTPEntryParam> rootInputFiles)
        {
            if (rootInputFiles.Count() == 0) return BadRequest("Нет файлов для добавления");
            if (string.IsNullOrWhiteSpace(pathId)) return BadRequest("Не указан идентификатор пути");
            if (!_idProvider.IsIdValid(pathId)) return BadRequest("Неверный идентификатор пути");
            if (!_pathRepo.Exists(pathId)) return BadRequest("Путь не найден");

            var delFailed = _treeSaverService.SaveRootEntriesWithoutChildren(pathId, rootInputFiles);

            return Ok("Все файлы успешно добавлены." + (delFailed > 0 ? $" Не удалось удалить файлов: {delFailed}" : ""));
        }

        /// <summary>
        /// Индексация деревьев файлов из ZIP архивов
        /// </summary>
        /// <param name="fileTree">Объект, хранящий дерево файлов и корень</param>
        /// <returns>200OK если индексация файлов прошла без ошибок</returns>
        [HttpPost("AddFileTree")]
        public IActionResult AddFileTree([FromQuery]string pathId, [FromBody]FTPEntriesTreeParam entries)
        {
            if (!_idProvider.IsIdValid(pathId)) return BadRequest("Неверный идентификатор пути");
            if (!_pathRepo.Exists(pathId)) return BadRequest("Путь не найден");
            var sw = new Stopwatch();
            _treeSaverService.SaveFTPEntriesTree(pathId, entries);
            _logger.Log($"Сохранил дерево файлов {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
            sw.Stop();
            return Ok("");
        }
        

        /// <summary>
        /// Возвращает все вновь созданные или измененные файлы
        /// </summary>
        /// <returns>Список файлов</returns>
        [HttpGet]
        public ListResponse<FTPEntry> Get([FromQuery]FilterOptions options)
        {
            return new ListResponse<FTPEntry>()
            {
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
            if (string.IsNullOrWhiteSpace(pathId)) return BadRequest("Необходимо указать идентификатор пути.");
            if (!_idProvider.IsIdValid(pathId)) return BadRequest("Неверный формат идентификатора пути");

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
            return new ListResponse<TenderPlanFileToIndexViewmodel>()
            {
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
            if (string.IsNullOrWhiteSpace(pathId)) return BadRequest("Необходимо указать идентификатор пути");
            if (!_idProvider.IsIdValid(pathId)) return BadRequest("Неверно указан идентификатор пути");

            var states = state.HasValue ? new StateFile[] { state.Value } : new StateFile[] { StateFile.New, StateFile.Modified };
            return new ListResponse<FTPEntry>()
            {
                Count = _entryRepo.CountAll(),
                Data = _entryRepo.GetByFileStateAndPath(options.Skip, options.Take, pathId, true, states).ToArray()
            };
        }

    }
}
