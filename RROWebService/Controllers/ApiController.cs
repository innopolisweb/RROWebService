using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;
using RROWebService.Models.ObjectModel;
using RROWebService.Services;

namespace RROWebService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly CompetitionContext _dbContext;

        public ApiController(CompetitionContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public string Authorize(string judgeId, string pass)
        {
            var getJudge = from judge in _dbContext.Judges
                where judge.Id == judgeId
                select judge;

            var jugde = getJudge.First();
            if (jugde.Pass.Equals(pass))
            {
                var token = JudgeAuthorizationFactory.AuthorizeOrRenew(judgeId, pass);
                return token.ToString();
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized).ToString();
        }

        [HttpGet]
        public IList<RROTeam> Teams(int? polygon)
        {
            var query = from it in _dbContext.Teams
                where polygon == null || it.Polygon == polygon
                select it;

            return query.ToList();
        }
        

        [HttpPost]
        public async Task<HttpResponseMessage> AddTeams([FromBody] IList<RROTeam> teams)
        {
            _dbContext.Teams.AddRange(teams);
            await _dbContext.SaveChangesAsync();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}