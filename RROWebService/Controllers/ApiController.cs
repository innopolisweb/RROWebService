using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
        public IActionResult Authorize(string judgeId, string pass)
        {
            var getJudge = from judge in _dbContext.Judges
                where judge.Id == judgeId
                select judge;

            if (!getJudge.Any())
            {
                return Unauthorized();
            }
            var jugde = getJudge.First();
            if (!jugde.Pass.Equals(pass)) return Unauthorized();

            var token = JudgeAuthorizationFactory.AuthorizeOrRenew(judgeId, pass);
            return Ok(token);

        }

        [HttpGet]
        public IList<RROTeam> Teams(int? polygon, string tour)
        {
            var query = from it in _dbContext.Teams
                where polygon == null || it.Polygon == polygon
                where tour == null || it.Tour == tour
                select it;

            return query.ToList();
        }

        [HttpGet]
        public ActionResult<RROJudge> Judge(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                return BadRequest();

            var judge = from it in _dbContext.Judges
                where it.Id == id
                select new RROJudge {Id = it.Id, Name = it.Name, Polygon = it.Polygon, Tour = it.Tour};

            return !judge.Any() ? null : judge.First();
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