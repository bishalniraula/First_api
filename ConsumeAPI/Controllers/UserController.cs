using ConsumeAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<UserViewModel> users = new List<UserViewModel>();

            var apiUrl = _config.GetValue<string>("baseAddress");
            var Url = apiUrl + ("/api/Account");
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.GetAsync(Url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<List<UserViewModel>>(data);
                    return View(user);

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


        ////post
        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserViewModel model)
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
                    HttpResponseMessage result = client.PostAsync(Url, content).Result;
                    string resultContent = result.Content.ReadAsStringAsync().Result;



                    var token = JsonConvert.DeserializeObject(resultContent);
                    HttpResponseMessage response = await client.PostAsync(Url, content);
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


                    if (response.IsSuccessStatusCode)
                    {

                        HttpContext.Session.SetString("UserName", model.Username);
                        return RedirectToAction("Index", "User");
                    }
                }
                TempData["name"] = " Invalid Id and Password ";
                return View();
            }
            
        }



        
        [HttpPost]

        public async Task<IActionResult> CreateAsync(UserViewModel model)
        {

            var apiUrl = _config.GetValue<string>("baseAddress");
            var Url = apiUrl + ("/api/Account");
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(Url, content);



                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserViewModel user = new UserViewModel();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
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
