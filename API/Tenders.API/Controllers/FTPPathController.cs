using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Services.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace TenderPlanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPPathController : ControllerBase
    {
        private readonly IPathService _pathService;
        private readonly IFTPPathRepo _repo;
        private readonly ILoggerService _logger;

        public FTPPathController(IPathService pathService, IFTPPathRepo Repo, ILoggerService Logger) : base()
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _repo = Repo ?? throw new ArgumentNullException(nameof(Repo));
            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
        }

        /// <summary>
        /// Метод для проверки валидности приходящей строки
        /// </summary>
        /// <param name="path">Путь к документу</param>
        /// <returns>200OK или ошибку, если что-то пошло не так</returns>
        [HttpGet("Check")]
        public IActionResult HelperCheckValidPath([FromQuery]string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Метод не может обрабатывать пустую строку");
            }
            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri u))
            {
                return BadRequest("Строка не валидна, укажите абслютный путь");
            }
            if (_repo.PathExistsByName(path))
            {
                return BadRequest("Путь уже существует");
            }
            else
                return Ok("");
        }

        /// <summary>
        /// Метод добавления уникального пути в базу данных
        /// </summary>
        /// <param name="path">Добавляемый путь</param>
        [HttpPost]
        public IActionResult Post([FromBody]FTPPathParam path)
        {
            var resultCheck = HelperCheckValidPath(path.Path) as ObjectResult;
            if (resultCheck.StatusCode == 200)
            {
                var id = _repo.Create(new FTPPath
                {
                    Path = path.Path,
                    Login = path.Login,
                    Password = path.Password
                });
                if (string.IsNullOrEmpty(id)) return StatusCode(500, "Не удалось добавить путь");
                return Created("/", id);
            }
            else
                return BadRequest(resultCheck.Value);


        }

        /// <summary>
        /// Метод, удаляющий путь из бд
        /// </summary>
        /// <param name="id">Идентификатор пути</param>
        /// <returns>200OK если путь удален успешно</returns>
        [HttpDelete]
        public IActionResult Delete([FromQuery]string id)
        {
            if (!_repo.Exists(id)) return BadRequest("Путь не найден");
            if (!_repo.Delete(id)) return StatusCode(500, "Не удалось удалить путь");
            return Ok("Путь удален");
        }

        /// <summary>
        /// Метод, меняющий флаг с true на false
        /// </summary>
        /// <param name="id">Идентификатор пути</param>
        /// <returns>200OK если флаг обновлен успешно</returns>
        [HttpPost("ChangeFlagActive")]
        public IActionResult ChangeFlagActive([FromBody]string id)
        {
            if (!_repo.Exists(id)) return NotFound("Элемент не существует");
            if (!_repo.ChangeActiveFlag(id, false)) return StatusCode(500, "Не удалось изменить флаг");

            return Ok("");
        }

        /// <summary>
        /// Метод для редактирования названия пути
        /// </summary>
        /// <param name="id"> идентификатор пути </param>
        /// <param name="path"> название пути, который нужно изменить </param>
        /// <returns></returns>
        [HttpPatch]
        public IActionResult Patch([FromBody]FTPPathParam path)
        {
            if (!_repo.Exists(path.Id)) return BadRequest("Путь не найден");
            if (string.IsNullOrEmpty(path.Path)) return BadRequest("Метод не может обрабатывать пустую строку");
            if (!Uri.TryCreate(path.Path, UriKind.Absolute, out Uri u)) return BadRequest("Строка не валидна, укажите абслютный путь");

            var oldPath = _repo.GetOne(path.Id);

            oldPath.Path = path.Path;
            oldPath.Login = path.Login;
            oldPath.Password = path.Password;

            if (!_repo.Update(oldPath)) return BadRequest("Данные не были обновлены");
        
            return Ok("");
        }

        /// <summary>
        /// Метод отображения активных (не удаленных) путей
        /// </summary>
        /// <returns>Список путей которые сейчас доступны для парсинга</returns>
        [HttpGet]
        public IActionResult Get([FromQuery]FilterOptions options)
        {
            if (options.PageSize <= 0)
            {
                _logger.Log($"Размер страницы установлен, как 0, устанавливаю размер страницы 10.");
                options.PageSize = 10;
            }

            return new JsonResult(new ListResponse<FTPPath>
            {
                Count = (int)_repo.CountAll(),
                Data = _repo.Get(options.Skip, options.Take).ToArray()
            });
        }

        /// <summary>
        /// Возращает боту очередной путь, который нужно индексировать
        /// </summary>
        /// <returns>Объект пути</returns>
        [HttpGet("Next")]
        public ActionResult<FTPPath> GetNext()
        {
            return _pathService.GetNotIndexedPath();
        }
    }
}
