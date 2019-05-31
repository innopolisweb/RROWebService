using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RROWebService.Authentication;
using RROWebService.Models.Categories.Oml;
using RROWebService.Models.ObjectModel;
using RROWebService.Models.ObjectModel.Abstractions;

namespace RROWebService.Controllers
{
    [Route("scoreboard")]
    public class ScoreBoardController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey("token"))
                return RedirectToAction("Index", "Authorization");

            var token = Request.Cookies["token"];
            var payload = JWTJudgeProvider.DecodeToken(token);

            var response = await new HttpClient().GetAsync($"http://localhost:5000/api/judge?token={token}");
            if (!response.IsSuccessStatusCode) return StatusCode(500);

            var judgeJson = await response.Content.ReadAsStringAsync();
            var judge = JsonConvert.DeserializeObject<RROJudge>(judgeJson);
            var teamsResponse = await new HttpClient()
                .GetAsync($"http://localhost:5000/api/teams?tour={payload.Tour}&polygon={judge.Polygon}");
            if (!teamsResponse.IsSuccessStatusCode) return StatusCode(500);

            var teamsJson = await teamsResponse.Content.ReadAsStringAsync();
            var teams = JsonConvert.DeserializeObject<List<RROTeam>>(teamsJson);
            if (!teams.Any()) return NotFound();

            var roundResponse = await new HttpClient().GetAsync("http://localhost:5000/api/currentround");
            if (!roundResponse.IsSuccessStatusCode) return StatusCode(500);

            var round = Int32.Parse(await roundResponse.Content.ReadAsStringAsync());
            var category = teams.First().Category;

            TempData["judge"] = judgeJson;
            TempData["teams"] = teamsJson;
            TempData["category"] = category;
            TempData["round"] = round.ToString();
            TempData["tour"] = payload.Tour.ToString();


            switch (category)
            {
                case "ОМЛ":
                    return RedirectToAction("Oml");
                default:
                    //TODO a lot of forms and tables
                    return NotFound();
            }
        }
    
        [Route("oml")]
        [HttpGet]
        public async Task<IActionResult> Oml()
        {
            var vm = new OmlViewModel();

            var round = Int32.Parse(TempData["round"].ToString());
            var tour = Int32.Parse(TempData["tour"].ToString());
            var category = (string)TempData["category"];
            var judge = JsonConvert.DeserializeObject<RROJudge>(TempData["judge"].ToString());
            var teams = JsonConvert.DeserializeObject<List<RROTeam>>(TempData["teams"].ToString());

            var preResultResponse = await new HttpClient()
                .GetAsync($"http://localhost:5000/api/omlpreresults?polygon={judge.Polygon}");
            if (!preResultResponse.IsSuccessStatusCode) return StatusCode(500);

            var preResultTeamsJson = await preResultResponse.Content.ReadAsStringAsync();
            var preResultTeams =
                JsonConvert.DeserializeObject<List<OmlScore>>(preResultTeamsJson);

            foreach (var team in teams)
            {
                var temp = preResultTeams.Find(t => t.TeamId == team.TeamId);
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

            vm.JudgeName = judge.JudgeName;
            vm.Polygon = judge.Polygon;
            vm.Category = category;
            vm.CurrentRound = round;

            ViewData["Polygon"] = vm.Polygon;
            ViewData["Tour"] = tour;
            ViewData["Category"] = vm.Category;
            ViewData["JudgeName"] = vm.JudgeName;
            ViewData["Round"] = round;

            return View(vm);
        }
    }
}