using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApp.Models
{
  public class TodoItem
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id {get;set;}
    public string Description {get; set;}
    public bool IsCompleted {get; set;}
  }
}