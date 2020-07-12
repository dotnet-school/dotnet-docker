using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;

namespace TodoApp.Controllers
{
  [ApiController]
  [Route("/api/[controller]")]
  public class TodosController : ControllerBase
  {
    private static IList<TodoItem> fakeItems = new List<TodoItem>()
    {
      new TodoItem(){Id = "one", Description = "task one", IsCompleted = true},
      new TodoItem(){Id = "two", Description = "task two", IsCompleted = false},
      new TodoItem(){Id = "three", Description = "task three", IsCompleted = false}
    };
    
    [HttpGet]
    public IEnumerable<TodoItem> GetAll()
    {
      return fakeItems; 
    }    
    [HttpGet("{id}")]
    public TodoItem GetById(string id)
    {
      return fakeItems.First(item => item.Id == id); 
    }
    
    [HttpPost]
    public ActionResult CreateItem(TodoItem data)
    {
      data.Id = $"task-{fakeItems.Count}";
      fakeItems.Add(data);
      return CreatedAtAction("GetById", new {Id = data.Id}, data);
    }
    
    [HttpPut("{id}")]
    public ActionResult GetById(string id, TodoItem data)
    {
      if (id != data.Id) return BadRequest("Ids in path and data do not match");
      
      var item = fakeItems.First(item => item.Id == id);
      if (item == null) return NotFound();

      item.Description = data.Description;
      item.IsCompleted = data.IsCompleted;

      return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTas(string id)
    {
      var item = fakeItems.First(item => item.Id == id);
      if (item == null) return NotFound();
      fakeItems.Remove(item);
      return Ok();
    }

  }
}