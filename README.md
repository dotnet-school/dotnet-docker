

To create a simple todo app with react, dotnet and mongodb refer : https://github.com/dotnet-school/dotnet-mongodb



### Setup project

- clone this repo  https://github.com/dotnet-school/dotnet-mongodb 

- Run mongo as docker container : 

  ```
  docker run  -i -v $PWD/db:/data/db -p 27017:27017 mongo:3.6.18
  ```

- Run our app 

  ```
  dotnet run
  ```

- Check if everything is working : https://localhost:5001/



### Pass connection string as environment variable

- Currently our docker connection string is defined in appsettings.json as : 

  ```yaml
  "MongoSettings": {
    "ConnectionString": "mongodb://localhost:27017", # This should come from env variable
    "DbName": "dotnet-mongo-demo",                   # This should come from env variable
    "TodoCollection": "Todo"
  },
  ```

  

- We need to make the configuration string based on env variable. This will give us the flexibilty to set it at runtime.

- To do this, we will run the app with env variable that should override the settings in appsettings.json. Read this for more info on env var config provider : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#evcp

- In Program.cs, create a env prefix. Any env variable with the prefix `TODO_APP_`  will override the values in appsettings.json

  ```diff
  public static IHostBuilder CreateHostBuilder(string[] args) =>
  	Host.CreateDefaultBuilder(args)
  +		.ConfigureAppConfiguration((hostingContext, config) =>
  +		{
  +	    config.AddEnvironmentVariables(prefix: "TODO_APP_");
  +   })
      .ConfigureWebHostDefaults(webBuilder =>
      {
    	  webBuilder.UseStartup<Startup>();
      });
  }
  ```

- Now lets set the environment variable with the mongo connection string

  ```bash
  # note there is two _ between MongoSettings and ConnectionString
  export TODO_APP_MongoSettings__ConnectionString="mongodb://localhost:27017"
  ```

  - remove the connection string to test out configuration.

- Now remove the `ConnectionString` value form appsettings.json 

  ```DIFF
    "MongoSettings": {
  -   "ConnectionString": "mongodb://localhost:27017",
  +   "ConnectionString": "INJECT_AT_RUNTIME",
      "DbName": "dotnet-mongo-demo",
      "TodoCollection": "Todo"
    },
  ```

- and run the app again

  ```
  dotnet run
  ```

- Our app is now picking up connection string from environment variable.



### Create docker container for app

- Create a multi-stage docker file for compiling and running our app : 

  ```dockerfile
  # Stage 1: Use an image with sdk (so that we can compile and build app)
  FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
  WORKDIR /source
  
  # Run dotnet restore
  COPY *.csproj .
  RUN dotnet restore
  
  # Copy rest of project and publish app int /app directory
  COPY . .
  RUN dotnet publish -c release -o /app --no-restore
  
  # Stage 2: We do not need the sdk in final image, just runtime (smaller efficient image)
  FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
  WORKDIR /app
  
  # Copy files form previous stage 
  COPY --from=build /app .
  
  EXPOSE 80
  ENTRYPOINT ["dotnet", "01-todo-api.dll"]
  ```

- Create a `.dockerignore`. This works like `.gitignore`. It tells docker to ignore these files when we copy things into our docker image

  ```yaml
  # directories
  **/bin/
  **/obj/
  **/out/
  
  #ides
  **/.idea
  **/.vscode
  
  # files
  Dockerfile*
  ```

- Now run the app 