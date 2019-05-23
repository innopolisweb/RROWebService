using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;

namespace RROWebService.Controllers
{
    public class AuthorizationController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (Request.Cookies.ContainsKey("token") && Request.Cookies.ContainsKey("judgeid"))
            {
                var judgeId = Request.Cookies["judgeid"];
                var response =
                    await new HttpClient().GetAsync($"http://localhost:5000/api/reauthorize?judgeId={judgeId}");

                if (response.IsSuccessStatusCode)
                {

                    var token = await response.Content.ReadAsStringAsync();
                    Response.Cookies.Delete("token");
                    Response.Cookies.Append("token", token, new CookieOptions
                    {
                        IsEssential = true,
                        Expires = DateTime.Now + TimeSpan.FromMinutes(40),
                    });
                    return RedirectToAction("Index", "ScoreBoard");
                }
            }

            return View(new AuthorizationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AuthorizationViewModel vm)
        {
            if (String.IsNullOrWhiteSpace(vm.JudgeId) || String.IsNullOrEmpty(vm.Pass))
            {
                vm.Secondary = true;
                vm.Error = false;
                vm.JudgeId = vm.JudgeId ?? "";
                vm.Pass = vm.Pass ?? "";
                return View(vm);
            }

            var judgeid = vm.JudgeId.Trim();
            var response =
                await new HttpClient().GetAsync($"http://localhost:5000/api/authorize?judgeId={judgeid}&pass={vm.Pass}");

            if (response.IsSuccessStatusCode)
            {

                var token = await response.Content.ReadAsStringAsync();
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTime.Now + TimeSpan.FromMinutes(40),
                });
                Response.Cookies.Append("judgeid", judgeid, new CookieOptions
                {
                    IsEssential = true,
                    Expires = DateTimeOffset.MaxValue
                });

                return RedirectToAction("Index", "ScoreBoard");
            }

            vm.Secondary = false;
            vm.Error = true;
            return View(vm);
        }
    }
}