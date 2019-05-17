using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RROWebService.Models.From1;
using RROWebService.Models.ObjectModel;

namespace RROWebService.Controllers
{
    public class ScoreBoardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (!Request.Cookies.ContainsKey("judgeid") || !Request.Cookies.ContainsKey("token"))
                return Unauthorized();

            var judgeid = Request.Cookies["judgeid"];

            //TODO a lot of forms and tables

            return RedirectToAction("Form1");
        }

        [HttpGet]
        public async Task<IActionResult> Form1()
        {
            var vm = new Form1ViewModel();

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

            var vmAdapted = from team in teamList
                select new Form1Item { Team = team.Id, Category = team.CategoryId };

            vm.JudgeName = judge.Name;
            vm.Polygon = judge.Polygon;
            vm.Tour = judge.Tour;
            vm.Items = vmAdapted.ToList();
            vm.Category = vm.Items.Count == 0 ? "unknown" : vm.Items.First().Category;

            ViewData["Polygon"] = vm.Polygon;
            ViewData["Tour"] = vm.Tour;
            ViewData["Category"] = vm.Category;
            ViewData["JudgeName"] = vm.JudgeName;

            return View(vm);
        }
    }
}