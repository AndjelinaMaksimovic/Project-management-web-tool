using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly AppDatabaseContext _databaseContext;

        public ItemController(AppDatabaseContext context)
        {
            _databaseContext = context;
        }

        [HttpGet("GetItems")]
        public ActionResult<IEnumerable<Item>> GetItems()
        {
            return _databaseContext.Items.ToList();
        }

        [HttpPost("AddItem")]
        public void AddItem(Item item)
        {
            _databaseContext.Items.Add(item);
            _databaseContext.SaveChanges();
        }

        [HttpPost("RemoveItem/{id}")]
        public async Task<ActionResult<Item>> RemoveItem(int id)
        {
            var item = await _databaseContext.Items.FindAsync(id);

            if (item == null) return NotFound();

            _databaseContext.Items.Remove(item);
            _databaseContext.SaveChanges();

            return item;
        }

        [HttpPut("UpdateItem/{id}")]
        public async Task<ActionResult<Item>> UpdateItem(int id, Item newItem)
        {
            var item = await _databaseContext.Items.FindAsync(id);

            if (item == null) return NotFound();

            item.Desc = newItem.Desc;
            item.Date = newItem.Date;

            _databaseContext.SaveChanges();

            return item;
        }
    }
}
