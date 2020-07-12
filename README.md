

To create a simple todo app with react and dotnet refer : https://github.com/dotnet-school/dotnet-react

Refer official doc : https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio



### Steps to add mongo

- Add mongo driver Nuget package
- create mongo configuration in app settings
- create model for mongo settings
- wire mongo setting with dependency injection in Startup.cs
- create a singleton service that uses mongo client
- inject settings in service and initialize mongo client
- wire controller to use the service
- update model for mappign mongo ids



### Install mongodb on local machine

- install mongodb and make sure it works on local machine

- use a gui tool like https://robomongo.org/

- We can instead use docker to install mongo db in single step and remove image when we are done : 

  ```bash
  docker run  -i -v $PWD/db:/data/db -p 27017:27017 mongo:3.6.18
  ```

  - this will start a docker container that listens to port `27017`
  - take a note of the db directory `$PWD/db`. If you change this, you will not see the data you saved earlier.
  - you can run this as a backgroudn task (daemon mode) using flag `-d`
  - connect with db to make sure our db is up and running 



### Setup project

- clone this repo  https://github.com/dotnet-school/dotnet-react

- Creating a servivce

  - Currenlty this has all fake logic in controller, we will move this logic to service 
  - Then we will make this serice use mongo client to save and fetch data from database

- Create a service :  `TodoApp/Services/TodoService.cs`

  ```c#
  using System.Collections.Generic;
  using System.Linq;
  using MongoDB.Driver;
  using TodoApp.Models;
  
  namespace TodoApp.Services
  {
    public class TodoService
    {
    }
  }
  ```

- Now wire this with the dependency injection in `Startup.cs` : 

  ```diff
  public void ConfigureServices(IServiceCollection services)
  {
  	services.AddControllersWithViews();
  +	services.AddSingleton<TodoService>();
  ```

- Inject the service in controller: 

  ```diff
  public class TodosController : ControllerBase
  {
  
  +		private TodoService _todoService;
  +		public TodosController(TodoService service)
  +		{
  +			_todoService = service;
  +		}
  
  [HttpGet]
  ```

- Now just move all code for fake data items from controller to service: 

  controller: 

  ```c#
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
  ```

  service: 

  ```c#
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
  ```

  

### Setup Mongo with .NET project

- Add mongo driver Nuget package

  ```bash
  cd TodoApp
  dotnet add package MongoDB.Driver
  ```
	TodoApp.csproj:
  ```diff
     <ItemGroup>
       <PackageReference Include="Microsoft.AspNetCore.SpaServices.Extensions" Version="3.1.0" />
  +    <PackageReference Include="MongoDB.Driver" Version="2.10.4" />
     </ItemGroup>
  
     <ItemGroup>
  ```

  
  
- Create the mongo db settings in `appsettings.Development.json`

  ```yaml
    "MongoSettings": {
      "ConnectionString": "mongodb://localhost:27017",
      "DbName": "dotnet-mongo-demo",
      "TodoCollection": "Todo"
    }
  ```

- Create a model class that represents the settings.

  - dotnet will load settings in object of this model 

  - our service will recive this model to configure the mongo client

    ```c#
    namespace TodoApp.Models
    {
      public class MongoSettings
      {
        public string ConnectionString { get; set; }
        public string DbName { get; set; }
        public string TodoCollection { get; set; }
      }
    }
    ```

  

- Wire the settings with DI in `TodoApp/Startup.cs`

  ```diff
  +using Microsoft.Extensions.Options;
  +using TodoApp.Models;
  
  public void ConfigureServices(IServiceCollection services)
  {
  
    services.AddControllersWithViews();
  
  + // This will map the model class to the section in appsettings.json
  + services.Configure<MongoSettings>(Configuration.GetSection(nameof(MongoSettings)));
  
  + // This will inject the values from appsettings into model instance
  + services.AddSingleton<MongoSettings>(s => s.GetRequiredService<IOptions<MongoSettings>>().Value);
  
    services.AddSpaStaticFiles(configuration =>
  ```



- Inject the settings into service: 

  - We wil now inject these settings from `appsettings.json` into our service

    ```diff
    + using MongoDB.Driver;
    using TodoApp.Models;

    namespace TodoApp.Services
    {
      public class TodoService
      {
    +    private IMongoCollection<TodoItem> _todosCollection;
  +    public TodoService(MongoSettings settings)
    +    {
    +      _todosCollection = new MongoClient(settings.ConnectionString)
    +              .GetDatabase(settings.DbName)
    +              .GetCollection<TodoItem>(settings.TodoCollection);
    +    }
      }
    }
  ```

  

- Update model for mongo 

  ```diff
  +using MongoDB.Bson;
  +using MongoDB.Bson.Serialization.Attributes;
  +
   namespace TodoApp.Models
   {
     public class TodoItem
     {
  +    // Mongo uses BsonId for keys
  +    [BsonId]
  +    [BsonRepresentation(BsonType.ObjectId)]
       public string Id {get;set;}
  
       public string Description {get; set;}
       public bool IsCompleted {get; set;}
     }
  ```

  

- Now update service to read/write using mongo collectoins instead of im memory list

  ```c#
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
  ```

  

