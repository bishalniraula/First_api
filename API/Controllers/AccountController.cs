using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {

           if(_context.users==null)
            {
                return NotFound();
            }

           return await  _context.users.ToListAsync();

        }
        [HttpGet("{id}")]
        
        public async Task <ActionResult<User>>GetUserById(int?id)
        {
            if(id==null)
            {
                return NotFound();
            }
                User user = await _context.users.FindAsync(id);
                if(user==null)
                {
                throw new Exception("user not found");
                  }
             return Ok(user);
               
            
        }
        [HttpPost]
        
        public async Task <ActionResult<User>>InsertUser(User user)
        {
           if(ModelState.IsValid)
           {
                _context.users.Add(user);
                _context.SaveChanges();
                return Ok();
           }
            return BadRequest();
        }


        



    }
}
