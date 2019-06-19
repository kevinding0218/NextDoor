
# NextDoor.Core - class library project
## How to create the project
1. Create web api project under _src_ subfolder
```
mkdir NextDoor.Core
cd .\NextDoor.Core\
mkdir src
dotnet new classlib -n NextDoor.Core -o src\NextDoor.Core
```
2. Create new solution file and add above project inside solution
```
dotnet new sln --name NextDoor.Core
dotnet sln add src/NextDoor.Core/NextDoor.Core.csproj
```
## Packages Included
### dev-01-mssql-repository-design
[Repository Design Related](https://deviq.com/repository-pattern/)
#### Newtonsoft.Json
```
dotnet add package Newtonsoft.Json
dotnet restore
```
#### [EntityFrameworkCore](https://www.learnentityframeworkcore.com/)
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet restore
```
### dev-02-mongo-repository-design
#### Mongo.Driver
```
dotnet add package MongoDB.Driver
dotnet restore
```
### dev-03-webapi-core-design
#### [Autofac]([https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html](https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html))
```
dotnet add package Autofac
dotnet restore
```
**Autofac Examples**:
1. *Reflection-based component*
- When using reflection-based components, Autofac automatically uses the constructor for your class with the most parameters that are able to be obtained from the container. 
- Any component type you register via RegisterType must be a concrete type. While components can expose abstract classes or interfaces as services, you can’t register an abstract/interface component.
	- Manually registration with Class as Interface ()
	```
	builder.RegisterType<MyClass>().As<IMyClass>()
	```
	<b>Explain</b>: Whenever you're looking for the Interface "IMyClass", return a instance of class "MyClass" in response
	- Parameters with Lambd	a Expression Component
	```
	builder.Register((c, p) =>
                 new ConfigReader(p.Named<string>("configSectionName"))).As<IConfigReader>();
    ```
	**Explain**: With lambda expression component registrations, rather than passing the parameter value _at registration time_ you enable the ability to pass the value _at service resolution time. 
			- Use TWO parameters to the registration delegate:
			- c = The current IComponentContext to dynamically resolve dependencies
			- p = An IEnumerable<Parameter> with the incoming parameter set
		```var reader = scope.Resolve<IConfigReader>(new NamedParameter("configSectionName", "sectionName"));```
**Explain**: The Resolve() methods accept [the same parameter types available at registration time]
2. *Automate registration by tracking assembly*
	```
	builder.RegisterAssemblyTypes(Assembly.Load(nameof(DemoLibrary)))
	.Where(t => t.Namespace.Contains("Utilities))
	.As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));
	```
	<b>Explain</b>: In "DemoLibrary" C# Class project, give me all the classes within 'Utilities' namespace
	and register them, then link them up to matching (I + class name) interface

 [**.Net Core self dependency injection lifetime types**](https://devblogs.microsoft.com/cesardelatorre/comparing-asp-net-core-ioc-service-life-times-and-autofac-ioc-instance-scopes/):
1. **AddSingleton** - creates a single instance throughout the application. It creates the instance for the first time and reuses the same object in the all calls.
> similar with using **InstancePerDependency()** in Autofac
- **InstancePerDependency()**
	- A unique instance will be returned from each object request.
2. **AddTransient** - created each time they are requested, a new instance is provided to every controller and every service. This lifetime works best for lightweight, stateless services.
> similar with using **InstancePerLifetimeScope()**/**InstancePerRequest()** in Autofac
- **InstancePerLifetimeScope()**
	- A component with per-lifetime scope will have at most a single instance per nested lifetime scope.
	- This is useful for objects specific to a single unit of work that may need to nest additional logical units of work. Each nested lifetime scope will get a new instance of the registered dependency.
	- For example, this type of lifetime scope is useful for Entity Framework DbContext objects (Unit of Work pattern) to be shared across the object scope so you can run transactions across multiple objects.
- **InstancePerRequest()**
	- Application types like ASP.NET Core naturally lend themselves to “request” type semantics. You have the ability to have a sort of “singleton per request.”
	- Instance per request builds on top of instance per matching lifetime scope by providing a well-known lifetime scope tag, a registration convenience method, and integration for common application types. Behind the scenes, though, it’s still just instance per matching lifetime scope.
3. **AddScoped** - same within a request, but different across different requests lifetime services are created once per request within the scope. It is equivalent to Singleton in the current scope.
> similar with using **SingleInstance()** in Autofac
- **SingleInstance()**
	- One instance is returned from all requests in the root and all nested scopes
> eg. in MVC it creates 1 instance per each http request but uses the same instance in the other calls within the same web request.
> 
#### Microsoft.Extension.Configuration
```
dotnet add package Microsoft.Extensions.Configuration
// dotnet add package Microsoft.Extensions.Configuration.FileExtensions
// dotnet add package Microsoft.Extensions.Configuration.Json
dotnet restore
```
#### Microsoft.AspNetCore.Http
```
dotnet add package Microsoft.AspNetCore
dotnet add package Microsoft.AspNetCore.Mvc
dotnet add package Microsoft.AspNetCore.Mvc.Core
dotnet add package Microsoft.AspNetCore.Mvc.DataAnnotations
dotnet add package Microsoft.AspNetCore.Mvc.Formatters.Json
dotnet restore
```
#### Microsoft.Extensions
```
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet restore
```
### dev-04-logging-design
#### [Serilog](https://serilog.net/)
```
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Extensions.Logging
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
dotnet restore
```
### dev-06-rabbitmq-design
#### [RawRabbit ](https://rawrabbit.readthedocs.io/en/master/)
```
<PackageReference Include="RawRabbit" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.DependencyInjection.ServiceCollection" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Enrichers.Attributes" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Enrichers.Polly" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Enrichers.MessageContext" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Enrichers.MessageContext.Subscribe" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.DependencyInjection.Autofac" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Enrichers.RetryLater" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Operations.Publish" Version="2.0.0-rc5" />
<PackageReference Include="RawRabbit.Operations.Subscribe" Version="2.0.0-rc5" />
dotnet restore
```
#### [Polly](https://github.com/App-vNext/Polly)
```
dotnet add package Polly
dotnet restore
```
#### [OpenTracing](https://www.olivercoding.com/2018-12-14-jaeger-csharp/)
```
dotnet add package OpenTracing
dotnet add package OpenTracing.Tag
dotnet restore
```
### dev-07-codereview-repositorydesign
- Refactor IReadRepository into INoSqlRepository and ISqlreadRepository
- Refacotr IIdentifiable into IIdIdentifiable and IGuidIdentifiable
- Add DataSeeder for MsSql
- Add Initilizer for MsSql
- Add extension for add Entity Framework based on dbContext
```
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="2.2.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="2.2.0" />
dotnet restore
```
### dev-08-redis-extension
- Add Redis as extension
```
<PackageReference Include="Microsoft.Extensions.Caching.Redis" Version="2.2.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="2.2.0" />
dotnet restore
```
### dev-09-first-microservice
#### Create a web api project and solution
1. Create web api project under _src_ subfolder
```
mkdir NextDoor.Services.Customers
cd .\NextDoor.Services.Customers\
mkdir src
dotnet new webapi -n NextDoor.Services.Customers -o src\NextDoor.Services.Customers
```
2. Create new solution file and add above project inside solution
```
dotnet new sln --name NextDoor.Services.Customers
dotnet sln add src/NextDoor.Services.Customers/NextDoor.Services.Customers.csproj
```
3. repeat above for additional micro services
	- NextDoor.Services.Customers
	- NextDoor.Services.Identity
	- NextDoor.Services.Accounts (Later)
	- NextDoor.Services.Billbooks (Later)
	- NextDoor.Services.Transactions (Later)
	- NextDoor.Services.Notifications (Later)
#### Re-organize a solution by adding all projects inside using Powershell script
- Listing the projects in a solution file
```
dotnet sln list
```
### dev-10-authentication-jwt-api
#### Add Jwt package
```
<PackageReference Include="Microsoft.AspNetCore.Authentication" Version="2.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="2.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Authorization" Version="2.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Hosting" Version="2.2.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity" Version="2.2.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="5.3.0" />
dotnet restore
```
#### [Refresh token](https://piotrgankiewicz.com/2017/12/07/jwt-refresh-tokens-and-net-core/)
#### [Cancelling token](https://piotrgankiewicz.com/2018/04/25/canceling-jwt-tokens-in-net-core/)
- what can we do in terms of canceling the active tokens? We have a few options:
	1. Remove token on the client side (e.g. local storage) – will do the trick, but doesn’t really cancel the token.
	2. Keep the token lifetime relatively short (5 minutes or so) – most likely we should do it anyway.
	3. Create a blacklist of tokens that were deactivated – this is what we are going to focus on.
- why are we use _IDistributedCache/Redis_?
	- use the Redis to store the deactivated tokens on an extremely fast caching server. 
	- If you're certain that you will hold your app on a single server and there will be only a single instance of your app, you can try with more default memory cahche
	- However, it's not reliable at first if you have more than a single instance, you may have a blacklist of expired or cancel tokens on server A or instance A then on instance B there is a different list, so whether your load balancer decides that the request goes to server A or B, you cannot be certain if the token was expired at A or B.
	– otherwise, when server goes down, you will lose all of the deactivated tokens blacklist being kept in a default server cache (not to mention the different data if each server would keep its own cache).
-------------------------------------------------------