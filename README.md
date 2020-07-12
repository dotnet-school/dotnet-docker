



- Create a project with

  ```bash
  # Create a react .net core boiler plate in current dir with namespace TodoApi
  dotnet new react -n TodoApp
  ```

- Add a .gitignore

  ```properties
  .fake/
  obj/
  bin/
  .idea/
  ```

- open in rider

  ```
  rider .
  ```

  - We have the react project in `ClientApp`

- Run project in watch mode :

  ```bash
  dotnet watch run
  ```

  - This should open url automatically : https://localhost:5001/

- Create a controller for todo api

  ```c#
  using System.Collections.Generic;
  using Microsoft.AspNetCore.Mvc;
  
  namespace TodoApp.Controllers
  {
    [ApiController]
    [Route("/api/[controller")]
    public class TodosController
    {
      [HttpGet]
      public IEnumerable<string> GetAll()
      {
        return new List<string>() {"task-one", "task-two"};
      }
    }
  }
  ```

  - check the api :  https://localhost:5001/api/todos

    ```yaml
    [
      "task-one",
      "task-two"
    ]
    ```

- Create a TODO Model

  ```c#
  namespace TodoApp.Models{
  	public class TodoItem{
  		public string Id {get;set;}
  		public string Description {get; set;}
  		public bool IsComplete {get; set;}
  	}
  }
  ```

- Now use the model in controller

  ```diff
  [HttpGet]
  - public IEnumerable<string> GetAll()
  + public IEnumerable<TodoItem> GetAll()
   {
  -  return new List<string>() {"task-one", "task-two"};
  +   return new List<TodoItem>()
  +   {
  +     new TodoItem(){Id = "one", Description = "task one", IsCompleted = true},
  +     new TodoItem(){Id = "two", Description = "task two", IsCompleted = false},
  +     new TodoItem(){Id = "three", Description = "task three", IsCompleted = false}
  +   }; 
   }
  ```

  - now check the api again : https://localhost:5001/api/todos

    ```yaml
    # Field names are automaticaaly changed from Pascal case to camelCase in json
    [
      {
        "description": "task one",
        "id": "one",
        "isCompleted": true
      },
      {
        "description": "task two",
        "id": "two",
        "isCompleted": false
      },
      {
        "description": "task three",
        "id": "three",
        "isCompleted": false
      }
    ]
    ```

  

