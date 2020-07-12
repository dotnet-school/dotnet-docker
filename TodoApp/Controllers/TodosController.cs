using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;

namespace TodoApp.Controllers
{
  [ApiController]
  [Route("/api/[controller]")]
  public class TodosController
  {
    [HttpGet]
    public IEnumerable<TodoItem> GetAll()
    {
      return new List<TodoItem>()
      {
              new TodoItem(){Id = "one", Description = "task one", IsCompleted = true},
              new TodoItem(){Id = "two", Description = "task two", IsCompleted = false},
              new TodoItem(){Id = "three", Description = "task three", IsCompleted = false}
      }; 
    }
  }
}