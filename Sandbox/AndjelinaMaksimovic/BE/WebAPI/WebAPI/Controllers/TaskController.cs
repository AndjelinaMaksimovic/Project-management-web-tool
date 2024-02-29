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

        // Read all - daje sve taskove koji se nalaze u bazi
        [HttpGet("GetItems")]
        public ActionResult<IEnumerable<Item>> GetTasks()
        {
            return _databaseContext.Tasks.ToList();
        }

        // 
        [HttpPost("AddItem")]
        public void AddTask(Task task)
        {
            _databaseContext.Tasks.Add(task);
            _databaseContext.SaveChanges();
        }


        // Delete - brisanje taska po ID-u
        [HttpPost("RemoveItem/{id}")]
        public async Task<ActionResult<Item>> RemoveItem(int id)
        {
            var task = await _databaseContext.Tasks.FindAsync(id);

            if (task == null) return NotFound();

            _databaseContext.Items.Remove(task);
            _databaseContext.SaveChanges();

            return task;
        }

        // Update - azuriranje taska po ID-u
        [HttpPut("UpdateItem/{id}")]
        public async Task<ActionResult<Item>> UpdateTask(int id, Task newTask)
        {
            var task = await _databaseContext.Tasks.FindAsync(id);

            if (task == null) return NotFound();

            task.Desc = newItem.Desc;
            task.Date = newItem.Date;

            _databaseContext.SaveChanges();

            return task;
        }
    }
}
