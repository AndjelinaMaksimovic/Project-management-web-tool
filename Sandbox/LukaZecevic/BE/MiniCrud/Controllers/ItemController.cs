using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCrud.Models;
using System.Reflection.Metadata;

namespace MiniCrud.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    private readonly AppDbContext _dbc;

    public ItemController(AppDbContext dbc)
    {
        _dbc = dbc;
    }

    // Create
    // POST: api/Item
    [HttpPost]
    public ActionResult<Item> AddItem(Item item)
    {
        _dbc.Add(item);
        _dbc.SaveChanges();
        return Ok();
    }

    // Read
    // GET: api/Item
    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetItems() => _dbc.Items.ToArray();

    //// GET: api/Item/1
    //[HttpGet("{id}")]
    //public ActionResult<Item> GetItem(long id)
    //{
    //    Item? i = _dbc.Items.Find(id);
    //    if (i == null)
    //        return NotFound();
    //    return i;
    //}


    // Update
    // PUT: api/Item/1
    [HttpPut("{id}")]
    public IActionResult UpdateItem(long id, Item item)
    {
        Item? i = _dbc.Items.Find(id);
        if (i == null)
            return NotFound();
        i.content = item.content;
        _dbc.SaveChanges();

        return Ok();
    }

    // Delete
    // DELETE: api/Item/1
    [HttpDelete("{id}")]
    public IActionResult DeleteItem(long id)
    {
        Item? i = _dbc.Items.Find(id);
        if (i == null)
            return NotFound();
        _dbc.Remove(i);
        _dbc.SaveChanges();
        return Ok();
    }
}
