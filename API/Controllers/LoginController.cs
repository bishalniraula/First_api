using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public LoginController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task <IActionResult> Login(User user)
        {
            IActionResult response = null;
            User _user = AuthenticateUser(user);
            if (_user != null)
            {
                var token = GenerateTokem(user);
                return Ok(new { token = token });
               

            }
            return response;
        }
        private  User? AuthenticateUser(User user)
        {
            User? _user = null;
            _user =  _context.users.Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();
            if (_user != null)
            {
                return new User { Username = user.Username };
            }

            return _user; 
        }
        private string GenerateTokem(User user)
        {
            SymmetricSecurityKey securitykey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            SigningCredentials credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(_configuration["Jwt:issuer"], _configuration["Jwt:audience"],
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials

                );
            string str= new JwtSecurityTokenHandler().WriteToken(token);
               return str;
//            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}