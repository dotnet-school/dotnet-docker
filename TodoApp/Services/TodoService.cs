using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using TodoApp.Models;

namespace TodoApp.Services
{
  public class TodoService
  {
    private IMongoCollection<TodoItem> _todosCollection;

    public TodoService(MongoSettings settings)
    {
      _todosCollection = new MongoClient(settings.ConnectionString)
              .GetDatabase(settings.DbName)
              .GetCollection<TodoItem>(settings.TodoCollection);
    }

    public IEnumerable<TodoItem> GetAll()
    {
      return _todosCollection.Find(t => true).ToList();
    }

    public TodoItem GetById(string id)
    {
      return _todosCollection.Find(t => t.Id == id).First();
    }

    public TodoItem UpdateItem(TodoItem data)
    {
      _todosCollection.ReplaceOne(t => t.Id == data.Id, data);
      return data;
    }

    public TodoItem CreateItem(TodoItem data)
    {
      _todosCollection.InsertOne(data);
      return data;
    }

    public void Delete(TodoItem item)
    {
      _todosCollection.DeleteOne(t => t.Id == item.Id);
    }
  }
}