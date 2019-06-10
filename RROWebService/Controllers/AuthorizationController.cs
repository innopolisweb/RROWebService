using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RROWebService.Models;

namespace RROWebService.Controllers
{
    public class AuthorizationController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey("token"))
                return View(new AuthorizationViewModel());

            var token = Request.Cookies["token"];
            var response = await new HttpClient().GetAsync($"https://rro.azurewebsites.net/api/checktoken?token={token}");
            var content = await response.Content.ReadAsStringAsync();

            if (content == "Valid") return RedirectToAction("Index", "ScoreBoard");
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

            var judgeId = vm.JudgeId.Trim();
            var pass = vm.Pass;
            var passBytes = Encoding.UTF8.GetBytes(pass);
            var passHashBytes = MD5.Create().ComputeHash(passBytes);
            var passHash = Convert.ToBase64String(passHashBytes);

            var response = await new HttpClient().GetAsync($"https://rro.azurewebsites.net/api/authorize?judgeId={judgeId}&pass={passHash}&serviceId=webApp");
            if (!response.IsSuccessStatusCode)
            {
                vm.Secondary = false;
                vm.Error = true;
                return View(vm);
            }

            var token = await response.Content.ReadAsStringAsync();
            Response.Cookies.Append("token", token, new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.MaxValue
            });
            return RedirectToAction("Index", "ScoreBoard");
        }
    }
}