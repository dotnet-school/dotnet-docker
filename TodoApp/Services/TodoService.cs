using System.Collections.Generic;
using System.Linq;
using TodoApp.Models;

namespace TodoApp.Services
{
  public class TodoService
  {
    private static IList<TodoItem> fakeItems = new List<TodoItem>()
    {
            new TodoItem() {Id = "one", Description = "task one", IsCompleted = true},
            new TodoItem() {Id = "two", Description = "task two", IsCompleted = false},
            new TodoItem() {Id = "three", Description = "task three", IsCompleted = false}
    };

    public IEnumerable<TodoItem> GetAll()
    {
      return fakeItems;
    }

    public TodoItem GetById(string id)
    {
      return fakeItems.First(item => item.Id == id);
    }

    public TodoItem UpdateItem(TodoItem data)
    {
      var item = fakeItems.First(item => item.Id == data.Id);
      item.Description = data.Description;
      item.IsCompleted = data.IsCompleted;
      return item;
    }

    public TodoItem CreateItem(TodoItem data)
    {
      data.Id = $"task-{fakeItems.Count}";
      fakeItems.Add(data);
      return data;
    }

    public void Delete(TodoItem item)
    {
      fakeItems.Remove(item);
    }
  }
}