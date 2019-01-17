using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Tenders.API.DAL.Interfaces;
using Tenders.API.Models;
using Tenders.API.Parameters;

namespace Tenders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderPlanIndexController : ControllerBase
    {
        private readonly ITenderPlanIndexRepo _indexRepo;

        public TenderPlanIndexController(ITenderPlanIndexRepo indexRepo)
        {
            _indexRepo = indexRepo ?? throw new System.ArgumentNullException(nameof(indexRepo));
        }

        /// <summary>
        /// </summary>
        /// <returns>Список всех файлов в индексе</returns>
        [HttpGet]
        public ListResponse<TenderPlanIndex> Get([FromQuery] FilterOptions options)
        {
            return new ListResponse<TenderPlanIndex>()
            {
                Count = _indexRepo.CountAll(),
                Data = _indexRepo.Get(options.Skip, options.Take).ToArray()
            };
        }

        [HttpPost]
        public ActionResult Post([FromBody]IEnumerable<TenderPlanIndexParam> TenderPlanIndexes)
        {
            TenderPlanIndexes.AsParallel().ForAll(i =>
            {
                var filter = Builders<TenderPlanIndex>.Filter.Eq("TenderPlanId", i.TenderPlanId);
                
                if (_indexRepo.ExistsByExternalId(i.TenderPlanId))
                {
                    // Файл не был проиндексирован ранее
                    var newIndexedFile = new TenderPlanIndex
                    {
                        FTPFileId = i.FTPFileId,
                        TenderPlanId = i.TenderPlanId,
                        RevisionId = i.RevisionId
                    };
                    _indexRepo.Create(newIndexedFile);
                }
                else
                {
                    // Файл уже есть в индексе
                    var indexedFile = _indexRepo.GetByExternalId(i.TenderPlanId);
                    if (indexedFile.RevisionId != i.RevisionId)
                    {
                        indexedFile.IsOutdated = true;
                        indexedFile.RevisionId = i.RevisionId;

                        _indexRepo.Update(indexedFile);   
                    }
                }
            });

            return Ok("Файлы проиндексированы.");
        }

    }
}
