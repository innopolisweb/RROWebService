using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;

namespace RROWebService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private CompetitionContext _dbContext;

        public ApiController(CompetitionContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet]
        public IEnumerable<string> M()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Api/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Api
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Api/5
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
