using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;
using RROWebService.Models.Categories.Oml;
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
                where judge.JudgeId == judgeId
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
                where it.JudgeId == id
                select new RROJudge {JudgeId = it.JudgeId, Name = it.Name, Polygon = it.Polygon, Tour = it.Tour};

            return !judge.Any() ? null : judge.First();
        }

        [HttpPost]
        public async Task<IActionResult> AddTeams([FromBody] IList<RROTeam> teams)
        {
            _dbContext.Teams.AddRange(teams);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> OmlResult([FromBody] OmlScore score)
        {
            if (!Request.Cookies.ContainsKey("judgeid") || !Request.Cookies.ContainsKey("token"))
                return Unauthorized();


            var teamInBase = from it in _dbContext.OMLScoreBoard
                where it.Round == score.Round
                where it.TeamId == score.TeamId
                select it;

            if (!teamInBase.Any())
            {
                await _dbContext.OMLScoreBoard.AddAsync(score);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            var record = teamInBase.First();
            if (record.Saved == 1) return Forbid();

            record.BlackBlockState = score.BlackBlockState;
            record.BlueBlockState = score.BlueBlockState;
            record.BrokenWall = score.BrokenWall;
            record.FinishCorrectly = score.FinishCorrectly;
            record.LiesCorrectly = score.LiesCorrectly;
            record.LiesIncorrectly = score.LiesIncorrectly;
            record.None = score.None;
            record.PartiallyCorrect = score.PartiallyCorrect;
            record.Saved = score.Saved;
            record.StaysCorrectly = score.StaysCorrectly;
            record.StaysIncorrectly = score.StaysIncorrectly;
            record.TimeMils = score.TimeMils;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}