using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Linq;
using TenderPlanAPI.Models;
using TenderPlanAPI.Parameters;
using TenderPlanAPI.Services;

namespace TenderPlanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPPathController : ControllerBase
    {
        private IPathService _pathService;
        private readonly IDBConnectContext db;

        public FTPPathController(IPathService pathService, IDBConnectContext Db) : base()
        {
            _pathService = pathService;
            db = Db ?? throw new ArgumentNullException(nameof(Db));
        }

        /// <summary>
        /// Метод для проверки валидности приходящей строки
        /// </summary>
        /// <param name="path">Путь к документу</param>
        /// <returns>200OK или ошибку, если что-то пошло не так</returns>
        [HttpGet("Check")]
        public IActionResult HelperCheckValidPath([FromQuery]string path)
        {

            var filter = Builders<FTPPath>.Filter.Eq("Path", path);
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("Метод не может обрабатывать пустую строку");
            }
            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri u))
            {
                return BadRequest("Строка не валидна, укажите абслютный путь");
            }
            if (db.FTPPath.Find(filter).Any())
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

                db.FTPPath.InsertOne(new FTPPath
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
        /// <param name="idParam">Идентификатор пути</param>
        /// <returns>200OK если путь удален успешно</returns>
        [HttpDelete]
        public IActionResult Delete([FromQuery]ObjectIdParam idParam)
        {

            var filter = Builders<FTPPath>.Filter.Eq("_id", idParam.Id);
            var s = db.FTPPath.DeleteOne(filter);
            if (s.DeletedCount <= 0)
                return BadRequest("Не найден элемент");

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

                var filter = Builders<FTPPath>.Filter.Eq("_id", path.Id);
                var updatePath = Builders<FTPPath>.Update.Set("Path", path.Path).Set("Login", path.Login);
                var s = db.FTPPath.UpdateOne(filter, updatePath);
                if (s == null || s.ModifiedCount <= 0)
                {
                    return BadRequest("Запись не обновлена");
                }
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


            var paths = db.FTPPath.Find(p => p.IsActive || (!string.IsNullOrWhiteSpace(options.Path) && p.Path.Equals(options.Path) && p.IsActive)).ToList();
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
