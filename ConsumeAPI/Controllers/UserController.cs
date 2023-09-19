using API.Models;
using ConsumeAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Text;
namespace ConsumeAPI.Controllers
{

    public class UserController : Controller
    {
        HttpClient client;
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel model)
        {

            var apiUrl = _config.GetValue<string>("baseAddress");
            var Url = apiUrl + ("/api/Login");
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {


                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage result = await client.PostAsync(Url, content);
                    string resultContent = await result.Content.ReadAsStringAsync();

                    object? token = JsonConvert.DeserializeObject(resultContent);

                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Username),
                        new Claim(ClaimTypes.Role, model.Role)
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,

                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,

                     new ClaimsPrincipal(claimsIdentity), properties);


                    if (result.IsSuccessStatusCode)
                    {
                        HttpContext.Session.SetString("token", resultContent);
                        return RedirectToAction("Index", "User");
                    }
                }
                TempData["name"] = " Invalid Id and Password ";
                return View();
            }

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var token = HttpContext.Session.GetString("token");
            if (token != null)
            {
                List<UserViewModel> users = new List<UserViewModel>();


               // string token = HttpContext.Session.GetString("token");


                var apiUrl = _config.GetValue<string>("baseAddress");
                var Url = apiUrl + ("/api/Account");
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.GetAsync(Url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("token", token);
                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;
                        var user = JsonConvert.DeserializeObject<List<UserViewModel>>(data);
                        return View(user);
                    }
                }
            }

            return View();
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }


       
        [Authorize]
        [HttpPost]

        public async Task<IActionResult> CreateAsync(UserViewModel model)
        {
            var token = HttpContext.Session.GetString("token");
            if (token != null)
            {
                var apiUrl = _config.GetValue<string>("baseAddress");
                var Url = apiUrl + ("/api/Account");
                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(Url, content);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("token", token);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }

                }
            }
            else if(token==null)
            {
                return RedirectToAction("Login");

            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            UserViewModel user = new UserViewModel();
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "/Account" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<UserViewModel>(data);

            }
            return View("Create", user);


        }

    }
}


/*
 * Login Controller Working Version
 * Authorize => Token, Unauthorized => 401 
 * Use urls and static values from config
 */


