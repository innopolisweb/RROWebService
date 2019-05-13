using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RROWebService.Models;
using RROWebService.Models.ObjectModel;

namespace RROWebService.Controllers
{
    public class HomeController : Controller
    {

        public HomeController(CompetitionContext context)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            var teams = new[] {new RROTeam {Polygon = 3, CategoryId = "1", Id = "11", Tour = "kv"}};
            var teamSerialized = JsonConvert.SerializeObject(teams);
            var response = await new HttpClient().PostAsync("http://localhost:5000/api/addteams", 
                new StringContent(teamSerialized, Encoding.UTF8, "application/json"));
            return Content(response.StatusCode.ToString());
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
