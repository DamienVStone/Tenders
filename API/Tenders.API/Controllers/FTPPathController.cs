using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using TenderPlanAPI.Services;
using Tenders.API.DAL.Interfaces;

namespace TenderPlanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPPathController : ControllerBase
    {
        private IPathService _pathService;
        private IFTPPathRepo _repo;

        public FTPPathController(IPathService pathService, IFTPPathRepo Repo) : base()
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _repo = Repo ?? throw new ArgumentNullException(nameof(Repo));
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
                if (id == Guid.Empty) return StatusCode(500, "Не удалось добавить путь");
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
        public IActionResult Delete([FromQuery]Guid id)
        {
            if (!_repo.Exists(id)) return BadRequest("Путь не найден");
            if (!_repo.Delete(id)) return StatusCode(500, "Не удалось удалить путь");
            return Ok("Путь удален");
        }

        /// <summary>
        /// Метод, меняющий флаг с true на false
        /// </summary>
        /// <param name="objId">Идентификатор пути</param>
        /// <returns>200OK если флаг обновлен успешно</returns>
        [HttpPost("ChangeFlagActive")]
        public IActionResult ChangeFlagActive([FromBody]Guid id)
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
        [HttpPut]
        public IActionResult Put([FromBody]FTPPathParam path)
        {
            if (!_repo.Exists(path.Id)) return BadRequest("Путь не найден");

            var resultCheck = HelperCheckValidPath(path.Path) as ObjectResult;
            if (resultCheck.StatusCode == 200)
            {
                var oldPath = _repo.GetOne(path.Id);

                oldPath.Path = path.Path;
                oldPath.Login = path.Login;
                oldPath.Password = path.Password;

                if (!_repo.Update(oldPath)) return StatusCode(500, "Не удалось обновить путь");
            }
            else
                return BadRequest(resultCheck.Value);

            return Ok("");
        }

        /// <summary>
        /// Метод отображения активных (не удаленных) путей
        /// </summary>
        /// <returns>Список путей которые сейчас доступны для парсинга</returns>
        [HttpGet]
        public IActionResult Get([FromQuery]FilterOptions options)
        {
            if (options.PageSize == 0) options.PageSize = 10;

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
