using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RROWebService.Models;
using RROWebService.Models.Categories.Oml;
using RROWebService.Models.ObjectModel;

namespace RROWebService.Controllers
{
    public class ScoreBoardController : Controller
    {
        private readonly CompetitionContext _dbContext;

        public ScoreBoardController(CompetitionContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Request.Cookies.ContainsKey("judgeid") || !Request.Cookies.ContainsKey("token"))
                return Unauthorized();

            var judgeid = Request.Cookies["judgeid"];

            //TODO a lot of forms and tables

            return RedirectToAction("Oml");
        }

        [HttpGet]
        public async Task<IActionResult> Oml()
        {
            var vm = new OmlViewModel();

            var judgeId = Request.Cookies["judgeid"];
            var response = await new HttpClient().GetAsync($"http://localhost:5000/api/judge?id={judgeId}");

            if (!response.IsSuccessStatusCode) return Unauthorized();

            var judgeSerialized = await response.Content.ReadAsStringAsync();
            var judge = JsonConvert.DeserializeObject<RROJudge>(judgeSerialized);

            var teams =
                await new HttpClient().GetAsync(
                    $"http://localhost:5000/api/teams?polygon={judge.Polygon}&tour={judge.Tour}");

            var teamsSerialized = await teams.Content.ReadAsStringAsync();
            var teamList = JsonConvert.DeserializeObject<List<RROTeam>>(teamsSerialized);

            var currentRound = _dbContext.CurrentRound.Last().Current;
            var overridedTeams = (from it in _dbContext.OMLScoreBoard
                where it.Round == currentRound
                where it.Polygon == judge.Polygon
                select it).ToList();

            foreach (var team in teamList)
            {
                var temp = overridedTeams.Find(t => t.TeamId == team.TeamId);
                if (temp == null)
                {
                    vm.Teams.Add(new OmlScoreViewModel {TeamId = team.TeamId});
                    continue;
                }
                vm.Teams.Add(new OmlScoreViewModel
                {
                    TeamId = temp.TeamId,
                    BlackBlockState = temp.BlackBlockState,
                    BlueBlockState = temp.BlueBlockState,
                    BrokenWall = temp.BrokenWall,
                    FinishCorrectly = temp.FinishCorrectly,
                    LiesCorrectly = temp.LiesCorrectly,
                    LiesIncorrectly = temp.LiesIncorrectly,
                    None = temp.None,
                    PartiallyCorrect = temp.PartiallyCorrect,
                    StaysCorrectly = temp.StaysCorrectly,
                    StaysIncorrectly = temp.StaysIncorrectly,
                    TimeMils = temp.TimeMils,
                    Saved = temp.Saved
                });
            }

            vm.JudgeName = judge.Name;
            vm.Polygon = judge.Polygon;
            vm.Tour = judge.Tour;
            vm.Category = teamList.Count == 0 ? "unknown" : teamList.First().CategoryId;
            vm.CurrentRound = currentRound;

            ViewData["Polygon"] = vm.Polygon;
            ViewData["Tour"] = vm.Tour;
            ViewData["Category"] = vm.Category;
            ViewData["JudgeName"] = vm.JudgeName;
            ViewData["Round"] = currentRound;

            return View(vm);
        }
    }
}