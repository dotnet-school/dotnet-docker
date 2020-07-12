using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers
{
  [ApiController]
  [Route("/api/[controller]")]
  public class TodosController : ControllerBase
  {
    private TodoService _todoService;

    public TodosController(TodoService service)
    {
      _todoService = service;
    }

    [HttpGet]
    public IEnumerable<TodoItem> GetAll()
    {
      return _todoService.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<TodoItem> GetById(string id)
    {
      var todoItem = _todoService.GetById(id);
      if (todoItem == null) return NotFound();
      return todoItem;
    }

    [HttpPost]
    public ActionResult CreateItem(TodoItem data)
    {
      TodoItem todoItem = _todoService.CreateItem(data);
      return CreatedAtAction("GetById", new {Id = todoItem.Id}, todoItem);
    }

    [HttpPut("{id}")]
    public ActionResult GetById(string id, TodoItem data)
    {
      if (id != data.Id) return BadRequest("Ids in path and data do not match");

      var item = _todoService.GetById(id);

      if (item == null) return NotFound();

      _todoService.UpdateItem(data);

      return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTas(string id)
    {
      var item = _todoService.GetById(id);
      if (item == null) return NotFound();

      _todoService.Delete(item);
      return Ok();
    }
  }
}