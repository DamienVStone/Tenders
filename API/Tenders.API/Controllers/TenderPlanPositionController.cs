using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Tenders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderPlanPositionController : ControllerBase
    {
        // GET: api/TenderPlanPosition
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TenderPlanPosition/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TenderPlanPosition
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/TenderPlanPosition/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
