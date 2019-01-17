using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Tenders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderPlanController : ControllerBase
    {
        // GET: api/TenderPlan
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TenderPlan/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TenderPlan
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/TenderPlan/5
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
