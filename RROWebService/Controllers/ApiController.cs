using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModelCore.Authentication;
using DataModelCore.DataContexts;
using DataModelCore.ObjectModel;
using DataModelCore.ObjectModel.Abstractions;
using DataModelCore.ObjectModel.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace RROWebService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {

        private static string RefreshTokenInternal(string token)
        {
            var payload = JWTJudgeProvider.DecodeToken(token);
            if (payload == null) return null;

            payload.OpenTime = DateTime.Now;
            payload.Expires = payload.OpenTime.AddMinutes(20);

            var newToken = JWTJudgeProvider.CreateToken(payload);
            return newToken;

        }

        private readonly CompetitionContext _dbContext;

        public ApiController()
        {
            _dbContext = new CompetitionContext();
        }

        [HttpGet]
        public ActionResult<string> Authorize(string judgeId, string pass, string serviceId)
        {
            var currentTour =
                (from ct in _dbContext.CurrentTour
                    select ct.Current).Last();

            var judge = (currentTour == 0
                ? (from tJudge in _dbContext.JudgesCv
                    where tJudge.JudgeId == judgeId
                    select tJudge).OfType<RROJudge>()

                : (from tJudge in _dbContext.JudgesFin
                    where tJudge.JudgeId == judgeId
                    select tJudge).OfType<RROJudge>()).FirstOrDefault();

            if (judge == null) return BadRequest(AuthenticationError.UserNotFound);

            if (currentTour == 0)
            {
                if (!((RROJudgeCv) judge).PassHash.Equals(pass))
                    return BadRequest(AuthenticationError.IncorrectPassword);
            }
            else
            {
                if (!((RROJudgeFin) judge).PassHash.Equals(pass))
                    return BadRequest(AuthenticationError.IncorrectPassword);
            }

            var payload = JudgePayload.Create(judge, currentTour, serviceId);
            var service =
                (from tService in _dbContext.Services
                    where tService.ServiceId == payload.Service
                    select tService).FirstOrDefault();
            if (service == null) return BadRequest(AuthenticationError.UnknownService);

            var tokenCv = JWTJudgeProvider.CreateToken(payload);
            return tokenCv;
        }

        [HttpGet]
        public ActionResult<TokenState> CheckToken(string token)
        {
            var payload = JWTJudgeProvider.DecodeToken(token);
            if (payload == null) return TokenState.Invalid;

            if (payload.Expires <= DateTime.Now) return TokenState.Expired;

            return TokenState.Valid;
        }

        [HttpGet]
        public ActionResult<string> RefreshToken(string token)
        {
            var newToken = RefreshTokenInternal(token);
            if (newToken == null) return BadRequest(TokenState.Invalid);
            return newToken;
        }

        [HttpGet]
        public ActionResult<int> CurrentRound(string category)
        {
            var roundQuery = from tRound in _dbContext.Rounds
                where tRound.Category == category
                where tRound.Current == 1
                orderby tRound.Round
                select tRound;
            if (!roundQuery.Any()) return -1;

            return roundQuery.Last().Round;
        }

        [HttpGet]
        public async Task<IActionResult> StartNewRound(string category)
        {
            if (!Request.Headers.ContainsKey("token")) return BadRequest(ScoreBoardPostError.TokenNotPassed);
            var token = Request.Headers["token"][0];

            var payload = JWTJudgeProvider.DecodeToken(token);
            if (payload == null) return BadRequest(ScoreBoardPostError.InvalidToken);

            if (payload.Status != "admin") return BadRequest(ScoreBoardPostError.AccessDenied);

            var roundQuery = from tRound in _dbContext.Rounds
                where tRound.Category == category
                orderby tRound.Round
                select tRound;

            foreach (var round in roundQuery)
                round.Current = 0;

            if (!roundQuery.Any())
                _dbContext.Rounds.Add(new CompetitionRound {Category = category, Round = 1, Current = 1});
            else
            {
                var lastRound = roundQuery.Last().Round;
                _dbContext.Rounds.Add(new CompetitionRound {Category = category, Round = lastRound + 1, Current = 1});
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public IActionResult CurrentTour()
        {
            var tour = _dbContext.CurrentTour.Last().Current;
            return Ok(tour);  
        }

        [HttpGet]
        public ActionResult<IEnumerable<OmlScore>> OmlPreResults(int tour, int? polygon, int? round, string teamId)
        {
            var teams = from team in _dbContext.OMLPreResults
                where team.Tour == tour
                select team;

            if (polygon != null)
            {
                teams = from team in teams
                    where team.Polygon == polygon
                    select team;
            }

            if (round != null)
            {
                teams = from team in teams
                    where team.Round == round
                    select team;
            }

            if (!String.IsNullOrWhiteSpace(teamId))
            {
                teams = from team in teams
                    where team.TeamId == teamId
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
        public ActionResult<RROJudge> Judge(string token)
        {
            var payload = JWTJudgeProvider.DecodeToken(token);

            if (payload == null) return BadRequest(TokenState.Invalid);

            var judgeId = payload.JudgeId;
            var tour = payload.Tour;

            switch (tour)
            {
                case 0:
                    var judgeCv = (from tJ in _dbContext.JudgesCv
                        where tJ.JudgeId == judgeId
                        select tJ).FirstOrDefault();

                    if (judgeCv == null) return BadRequest(TokenState.Invalid);
                    return new RROJudge
                    {
                        JudgeId = judgeCv.JudgeId,
                        Polygon = judgeCv.Polygon,
                        Status = judgeCv.Status,
                        JudgeName = judgeCv.JudgeName
                    };
                case 1:
                    var judgeFin = (from tJ in _dbContext.JudgesFin
                        where tJ.JudgeId == judgeId
                        select tJ).FirstOrDefault();

                    if (judgeFin == null) return BadRequest(TokenState.Invalid);
                    return new RROJudge
                    {
                        JudgeId = judgeFin.JudgeId,
                        Polygon = judgeFin.Polygon,
                        Status = judgeFin.Status,
                        JudgeName = judgeFin.JudgeName
                    };
                default:
                    return BadRequest(TokenState.Invalid);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> OmlPreResult([FromBody] OmlScore score)
        {
            if (!Request.Headers.ContainsKey("token"))
                return BadRequest(ScoreBoardPostError.TokenNotPassed);

            var token = Request.Headers["token"][0];
            var payload = JWTJudgeProvider.DecodeToken(token);

            if (payload == null) return BadRequest(ScoreBoardPostError.InvalidToken);
            if (payload.Expires <= DateTime.Now) return BadRequest(ScoreBoardPostError.TokenExpired);

            var judgeId = payload.JudgeId;
            if (score.JudgeId != judgeId)
                return BadRequest(ScoreBoardPostError.RecordPayloadMismatch);

            if (score.Saved == 1)
            {
                var hasNulls = score.RedBlockState == null || score.YellowBlockState == null ||
                               score.GreenBlockState == null || score.WhiteBlock1State == null ||
                               score.WhiteBlock2State == null || score.BlueBlockState == null ||
                               score.BattaryBlock1State == null || score.BattaryBlock2State == null ||
                               score.RobotState == null || score.Wall1State == null || score.Wall2State == null ||
                               score.Time1 == null || score.Time2 == null;
                if (hasNulls) return BadRequest(ScoreBoardPostError.InvalidModelContainsNulls);
            }

            var teamInBase = from it in _dbContext.OMLPreResults
                where it.Round == score.Round
                where it.TeamId == score.TeamId
                select it;

            if (!teamInBase.Any())
            {
                score.JudgeId = judgeId;
                await _dbContext.OMLPreResults.AddAsync(score);
                await _dbContext.SaveChangesAsync();
                var newToken = RefreshTokenInternal(token);
                return newToken;
            }

            var record = teamInBase.First();
            if (record.Saved == 1) return BadRequest(ScoreBoardPostError.OverwritingForbidden);

            record.JudgeId = judgeId;
            record.Tour = score.Tour;
            record.RedBlockState = score.RedBlockState;
            record.YellowBlockState = score.YellowBlockState;
            record.GreenBlockState = score.GreenBlockState;
            record.WhiteBlock1State = score.WhiteBlock1State;
            record.WhiteBlock2State = score.WhiteBlock2State;
            record.BlueBlockState = score.BlueBlockState;
            record.BattaryBlock1State = score.BattaryBlock1State;
            record.BattaryBlock2State = score.BattaryBlock2State;
            record.RobotState = score.RobotState;
            record.Wall1State = score.Wall1State;
            record.Wall2State = score.Wall2State;
            record.Time1 = score.Time1;
            record.Time2 = score.Time2;
            record.Saved = score.Saved;
            await _dbContext.SaveChangesAsync();
            var newToken2 = RefreshTokenInternal(token);
            return newToken2;
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