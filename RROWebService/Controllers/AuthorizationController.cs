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
        public IActionResult Index()
        {
            if (Request.Cookies.ContainsKey("token"))
                return Content(Request.Cookies["token"]);
            return View(new AuthorizationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AuthorizationViewModel vm)
        {
            if (String.IsNullOrWhiteSpace(vm.JudgeId) || String.IsNullOrEmpty(vm.Pass))
            {
                vm.Secondary = true;
                vm.Error = false;
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
                    Expires = DateTimeOffset.Now + TimeSpan.FromMinutes(20),
                });
                return Content(token);
            }

            vm.Secondary = false;
            vm.Error = true;
            return View(vm);
        }
    }
}