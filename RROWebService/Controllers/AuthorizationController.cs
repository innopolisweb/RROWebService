using System.Net;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;

namespace RROWebService.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(AuthorizationViewModel vm)
        {
            var response =
                new WebClient().DownloadString(
                    $"http://localhost:5000/api/authorize?judgeId={vm.JudgeId}&pass={vm.Pass}");

            return Content(response);
        }
    }
}