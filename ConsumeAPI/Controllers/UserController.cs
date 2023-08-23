using ConsumeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                string data= response.Content.ReadAsStringAsync().Result;
               users= JsonConvert.DeserializeObject<List<UserViewModel>>(data);
              
            }
            return View(users);
        }
    }
}
