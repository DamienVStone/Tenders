using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
            if (_repo.GetSinglePathByName(path)!=null)
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

                _repo.Create(new FTPPath
                {
                    Path = path.Path,
                    Login = path.Login,
                    Password = path.Password
                });
            }
            else
                return BadRequest(resultCheck.Value);

            return Ok("");
        }

        /// <summary>
        /// Метод, удаляющий путь из бд
        /// </summary>
        /// <param name="id">Идентификатор пути</param>
        /// <returns>200OK если путь удален успешно</returns>
        [HttpDelete]
        public IActionResult Delete([FromQuery]Guid id)
        {
            _repo.Delete(id);
            return Ok("Путь удален");
        }

        /// <summary>
        /// Метод, меняющий флаг с true на false
        /// </summary>
        /// <param name="objId">Идентификатор пути</param>
        /// <returns>200OK если флаг обновлен успешно</returns>
        [HttpPost("ChangeFlagActive")]
        public IActionResult ChangeFlagActive([FromBody]ObjectIdParam objId)
        {

            var filter = Builders<FTPPath>.Filter.Eq("_id", objId);
            var update = Builders<FTPPath>.Update.Set("IsActive", false);
            var s = db.FTPPath.UpdateOne(filter, update);
            if (s.ModifiedCount <= 0)
                return BadRequest("Не найден элемент");

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
            var resultCheck = HelperCheckValidPath(path.Path) as ObjectResult;
            if (resultCheck.StatusCode == 200)
            {
                var oldPath = _repo.GetOne(path.Id);

                oldPath.Path = path.Path;
                oldPath.Login = path.Login;
                oldPath.Password = path.Password;

                _repo.Update(oldPath);
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


            var paths = _repo.GetAll();
            return new JsonResult(new ListResponse<FTPPath>
            {
                Count = paths.Count(),
                Data = paths.Skip(options.Skip).Take(options.Take).ToArray()
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
