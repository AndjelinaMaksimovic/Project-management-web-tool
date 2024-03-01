using MarkoNenadovicAPI.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkoNenadovicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class ValuesController : ControllerBase
    {
        private dataContext _context;
        public ValuesController(dataContext context) {
            _context= context;
        }
        [HttpGet]
        public async Task<ActionResult<List<product>>> GetProduct()
        {
            return Ok(await _context.products.ToListAsync());

        }

        [HttpPost]
        public async Task<ActionResult<List<product>>> CreateProduct(product p)
        {
            _context.products.Add(p);
            await _context.SaveChangesAsync();
            return Ok(await _context.products.ToListAsync());

        }

        [HttpPut]
        public async Task<ActionResult<List<product>>> UpdateProduct(product p)
        {
            var prod= await _context.products.FindAsync(p.Id);
            if(prod == null)
            {
                return BadRequest("nije pronadjen");
            }
            
            prod.Name = p.Name;
            prod.price=p.price;
            await _context.SaveChangesAsync();

            return Ok(await _context.products.ToListAsync());

        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult<List<product>>> DeleteProduct(int id)
        {
            var prod = await _context.products.FindAsync(id);
            if (prod == null)
            {
                return BadRequest("nije pronadjen");
            }
            _context.products.Remove(prod);
            await _context.SaveChangesAsync();

            return Ok(await _context.products.ToListAsync());
        }

    }
}
