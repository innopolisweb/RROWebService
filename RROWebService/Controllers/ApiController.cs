using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Authentication;
using RROWebService.Models;
using RROWebService.Models.ObjectModel;
using RROWebService.Models.ObjectModel.Abstractions;

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
            var currentTour =
                (from ct in _dbContext.CurrentTour
                    select ct.Current).Last();

            switch (currentTour)
            {
                case 0:
                    var getJudgeCv = from judge in _dbContext.JudgesCv
                        where judge.JudgeId == judgeId
                        select judge;

                    if (!getJudgeCv.Any())
                    {
                        return BadRequest("User not found");
                    }

                    var judgeCv = getJudgeCv.First();
                    if (!judgeCv.Pass.Equals(pass)) return BadRequest("Incorrect password");

                    var payloadCv = JudgePayload.Create(judgeCv, 0);
                    var tokenCv = JWTJudgeProvider.CreateToken(payloadCv);
                    JWTJudgeFactory.AddToken(judgeCv.JudgeId, tokenCv, true);

                    return Ok(tokenCv);


                case 1:
                    var getJudgeFin = from judge in _dbContext.JudgesFin
                        where judge.JudgeId == judgeId
                        select judge;

                    if (!getJudgeFin.Any())
                    {
                        return BadRequest("User not found");
                    }

                    var judgeFin = getJudgeFin.First();
                    if (!judgeFin.Pass.Equals(pass)) return BadRequest("Incorrect password");

                    var payloadFin = JudgePayload.Create(judgeFin, 1);
                    var tokenFin = JWTJudgeProvider.CreateToken(payloadFin);
                    JWTJudgeFactory.AddToken(judgeFin.JudgeId, tokenFin, true);

                    return Ok(tokenFin);
                

                default:
                    return BadRequest("Server error");
            }
        }

        [HttpGet]
        public IActionResult CheckToken(string token)
        {
            JudgePayload payload;
            try
            {
                payload = JWTJudgeProvider.DecodeToken(token);
            }
            catch (Exception)
            {
                return Ok("Invalid");
            }

            if (payload.Expires <= DateTime.Now) return Ok("Invalid");

            var tokens = JWTJudgeFactory.GetAllTokensForJudge(payload.JudgeId);
            if (tokens.Any(t => t == token)) return Ok("Valid");
            return Ok("Invalid");
        }

        [HttpGet]
        public IActionResult CurrentRound()
        {
            var round = _dbContext.CurrentRound.Last().Current;
            return Ok(round);
        }

        [HttpGet]
        public ActionResult<IEnumerable<OmlScore>> OmlPreResults(int? polygon)
        {
            var teams = _dbContext.OMLScoreBoard.AsQueryable();

            if (polygon != null)
            {
                teams = from team in teams
                    where team.Polygon == polygon
                    select team;
            }

            return teams.ToList();
        }

        [HttpGet]
        public ActionResult<RROJudge> JudgeCv(string judgeId)
        {
            var judge = from tJudge in _dbContext.JudgesCv
                where tJudge.JudgeId == judgeId
                select tJudge;

            if (!judge.Any()) return NotFound(judgeId);

            return judge.First();
        }


        [HttpGet]
        public ActionResult<RROJudge> JudgeFin(string judgeId)
        {
            var judge = from tJudge in _dbContext.JudgesFin
                where tJudge.JudgeId == judgeId
                select tJudge;

            if (!judge.Any()) return NotFound(judgeId);

            return judge.First();
        }

        [HttpGet]
        public ActionResult<IEnumerable<RROTeam>> Teams(int tour, int? polygon, string category)
        {
            switch (tour)
            {
                case 0:
                    return TeamsCv(polygon, category);
                case 1:
                    return TeamsFin(polygon, category);
                default:
                    return BadRequest("Unknown tour parameter value");
            }
        }

        [HttpGet]
        public ActionResult<RROJudge> Judge(string token, string judgeId, int? tour)
        {
            var tJudgeId = judgeId;
            var tTour = tour ?? -1;
            if (!String.IsNullOrWhiteSpace(token))
            {
                JudgePayload payload;
                try
                {
                    payload = JWTJudgeProvider.DecodeToken(token);
                }
                catch (Exception e)
                {
                    return BadRequest("Can't resolve token: " + e.Message);
                }

                tJudgeId = payload.JudgeId;
                tTour = payload.Tour;
            }

            switch (tTour)
            {
                case 0:
                    var judgeCv = (from tJ in _dbContext.JudgesCv
                        where tJ.JudgeId == tJudgeId
                        select tJ).FirstOrDefault();

                    if (judgeCv == null) return Ok();
                    return judgeCv;
                case 1:
                    var judgeFin = (from tJ in _dbContext.JudgesFin
                        where tJ.JudgeId == tJudgeId
                        select tJ).FirstOrDefault();

                    if (judgeFin == null) return Ok();
                    return judgeFin;
                default:
                    return BadRequest("Invalid tour parameter value");
            }

        }

        [HttpPost]
        public async Task<IActionResult> OmlResult([FromBody] OmlScore score)
        {
            if (!Request.Cookies.ContainsKey("judgeid"))
                return Unauthorized();

            var judgeId = Request.Cookies["judgeid"];

            var teamInBase = from it in _dbContext.OMLScoreBoard
                where it.Round == score.Round
                where it.TeamId == score.TeamId
                select it;

            if (!teamInBase.Any())
            {
                score.JudgeId = judgeId;
                await _dbContext.OMLScoreBoard.AddAsync(score);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            var record = teamInBase.First();
            if (record.Saved == 1) return Forbid();

            record.JudgeId = judgeId;
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

        private ActionResult<IEnumerable<RROTeam>> TeamsCv(int? polygon, string category)
        {
            var request = from team in _dbContext.TeamsCv
                select team;

            if (polygon != null)
            {
                request = from tTeam1 in request
                    where tTeam1.Polygon == polygon
                    select tTeam1;
            }

            if (!String.IsNullOrWhiteSpace(category))
            {
                request = from tTeam2 in request
                    where tTeam2.Category == category
                    select tTeam2;
            }

            return request.ToList();
        }

        private ActionResult<IEnumerable<RROTeam>> TeamsFin(int? polygon, string category)
        {
            var request = from team in _dbContext.TeamsFin
                select team;

            if (polygon != null)
            {
                request = from tTeam1 in request
                    where tTeam1.Polygon == polygon
                    select tTeam1;
            }

            if (!String.IsNullOrWhiteSpace(category))
            {
                request = from tTeam2 in request
                    where tTeam2.Category == category
                    select tTeam2;
            }

            return request.ToList();
        }

    }
}