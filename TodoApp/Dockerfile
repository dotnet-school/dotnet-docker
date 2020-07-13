# Stage 1: Use an image with sdk (so that we can compile and build app)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# Our base image has dotnet sdk but not nodejs
# So we will instal nodejs to build our react project
# //github.com/nodesource/distributions/blob/master/README.md#deb
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs


# Run dotnet restore
COPY *.csproj .
RUN dotnet restore

# Copy rest of project and publish app int /app directory
COPY . .
RUN dotnet publish -c release -o /app --no-restore

# Stage 2: We do not need the sdk and nodejs in final image, just runtime (smaller efficient image)
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

# Copy files form previous stage 
COPY --from=build /app .

EXPOSE 80
ENTRYPOINT ["dotnet", "TodoApp.dll"]