using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrandS()
        {
            if (_context.Brands == null)
            {
                return NotFound();
            }
            return await _context.Brands.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrandById(int? id)
        {
            //if(_context.Brands == null)
            //{
            //    return NotFound();
            //}           //Brand brand = await _context.Brands.FindAsync(id);
            //return brand;

            Brand brand = await _context.Brands.Where(x => x.ID == id).FirstOrDefaultAsync();
            if (brand == null)
            {
                return NotFound();
            }
            return brand;

        }
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                //return Ok();
              return CreatedAtAction(nameof(GetBrandById), new { id = brand.ID },brand);
            }
            return BadRequest();


        }
        [HttpPut]
        public async Task<ActionResult> PutBrand(int id, Brand brand)
        {
            if (id != brand.ID)
            {
                return BadRequest();
            }
            _context.Brands.Entry(brand).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DBConcurrencyException ex)
            {
                if (!BrandAvailable(id))
                {
                    return NotFound();

                }
                else
                {
                    throw;
                }

            }
            return Ok();



        }
        private bool BrandAvailable(int id)
        {
            return (_context.Brands?.Any(x => x.ID == id)).GetValueOrDefault();
        }
        [HttpDelete("{id}")]
        public async Task <IActionResult>DeleteBrand(int id)
        
        {

            if(_context.Brands==null)
            {
                return NotFound();
            }
            Brand brand=await _context.Brands.Where(x=>x.ID == id).FirstOrDefaultAsync();   
            if(brand==null)
            {
                return NotFound();
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok();
        }



    }
}
