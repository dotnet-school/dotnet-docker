namespace TodoApp.Models
{
  public class MongoSettings
  {
    public string ConnectionString { get; set; }
    public string DbName { get; set; }
    public string TodoCollection { get; set; }
  }
}