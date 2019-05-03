####NextDoor.Core - class library project
#dotnet new classlib -n NextDoor.Core -o NextDoor.Core
#dotnet sln add .\NextDoor.Core\NextDoor.Core.csproj
#dotnet sln list
-------------------------------------------------------
###Packages Included

##Newtonsoft.Json
#dotnet add package Newtonsoft.Json
#dotnet restore

##Microsoft.EntityFrameworkCore
#dotnet add package Microsoft.EntityFrameworkCore
#dotnet restore

##Mongo.Driver
#dotnet add package MongoDB.Driver
#dotnet restore

##Autofac
#dotnet add package Autofac
#dotnet restore

##Microsoft.Extension.Configuration
#dotnet add package Microsoft.Extensions.Configuration
--#dotnet add package Microsoft.Extensions.Configuration.FileExtensions
--#dotnet add package Microsoft.Extensions.Configuration.Json
#dotnet restore
-------------------------------------------------------