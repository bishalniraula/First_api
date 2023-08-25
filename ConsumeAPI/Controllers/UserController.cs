using ConsumeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Mime;
using System.Text;

namespace ConsumeAPI.Controllers
{
    public class UserController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:1475/api");
        HttpClient client;
        public UserController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
        public ActionResult Index()
        {
            List<UserViewModel> users = new List<UserViewModel>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserViewModel>>(data);

            }
            return View(users);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(data, Encoding.UTF8, "Application/Json");
            HttpResponseMessage responseMessage = client.PostAsync(client.BaseAddress + "/Account", content).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserViewModel user = new UserViewModel();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account" + id).Result;
            if(response.IsSuccessStatusCode)
            {
                string data =response.Content.ReadAsStringAsync().Result;
                user=JsonConvert.DeserializeObject<UserViewModel>(data);
                
            }
            return View("Create", user);


        }

    }
}
