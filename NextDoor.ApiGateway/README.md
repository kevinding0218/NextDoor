## Use of RestEase (HTTP Call Redirection)
### Browse Registered Users
- Create a `IAdminService` interface which will be used as our RestEase redirection, define the way of Http Action and return type as same as what we will be used later on in our internal service.
	```
	[HttpGet]
    public async Task<IActionResult> Get([FromQuery] BrowseUserQuery query)
        => Collection(await _adminService.BrowseUsersAsync(query));
	```
	- RestEase will look for the appsettings.json config for where the service name mapping to which host and port, then in StartUp class, register the `IAdminService` with the config we defined `restEase.services.admin-service`
	```
	services.RegisterServiceForwarder<IAdminService>("admin-service");
	```
	- Inside of its controller action, define a action method by injecting and using `IAdminService`, whenever a http call reaches our API gateway, it will then redirect to our AdminService http get method.
	```
	[HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] BrowseUserQuery query)
        => Ok(await _dispatcher.QueryAsync(query));
	```
	- Then same approach of local dispatch will be executed within identity.service and return our IEnumerable<UserDto>.
## Using Serilog
### Installation
- Serilog.AspNetCore enables Serilog for ASP.NET Core, you have to add 
- Serilog.Settings.Configuration if you need to enable configuration from appsettings.json
- Serilog.Sinks.RollingFile enables you to log into rolling files your logs
```
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Settings.Configuration
dotnet add package Serilog.Sinks.RollingFile
```
### Configure appsetting.json
```
{
 "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
    {
       "Name": "RollingFile",
       "Args": {
          "pathFormat": "C:\\Temp\\log-{Date}.txt",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
       }
    }
   ],
   "Properties": {
      "Application": "Common feature in WebApi demo"
   }
 }
```
### Configuring Startup.cs
- read the configuration from appsettings.json
```c#
public Startup(IConfiguration configuration)
{
   // Init Serilog configuration
   Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
   Configuration = configuration;
}
```
- add Serilog to the LoggerFactory in the Configure method, so that Serilog override the default Microsoft Logger provided by the assembly: Microsoft.Extensions.Logging
```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
   // logging
   loggerFactory.AddSerilog();
   app.UseMvc();
 }
```
- Create logging example
```c#
public class DemoExceptionController : ControllerBase
{
   private readonly ILogger<DemoExceptionController> _logger;
   public DemoExceptionController(ILogger<DemoExceptionController> logger)
   {
      _logger = logger;
   }

   [HttpGet]
   public IEnumerable<string> Get()
   {
      try
      {
         _logger.LogInformation("Could break here :(");
         throw new Exception("bohhhh very bad error");
      }
      catch (Exception e)
      {
         _logger.LogError(e, "It broke :(");
      }
      return new string[] { "value1", "value2" };
   }
}
```
## Service discovery & Load balancing with Consul + Fabio
### Why
- Avoid putting too large objects on a RabbitMQ, try to keep them as small as possible, pass only information that's actually necessary for the other service to identity that something will not change or was created
### Consul
- Service registration centralized,each of the micro service will actually register to the consul, in `StartUp` we'll add some logic that says whenever our micro service is up, we want to call consul and register our micro service with particular data. If you scale your services horinzontally, like 10 or 50 instances, consul will randomly pick one of your service instances and hit that service to get the data.
- IP addresses is not a concern of our micro services, but a concern of the service registry.
```
//dotnet add package Consul
docker-compose -f docker-compose-consul-vault.yml up
// Consul will start at localhost:8500/ui
```
## Handling Async Requests
- In our API gateway BaseController, we have the `SendAsync` had the `GetContext<T>`, and what it does is to when you send the request to the API Gateway and create a new context, passes the new Guid as its Id. 
- Then we will use `await _busPublisher.SendAsync(command, context);` to send our command with the context we created to RabbitMQ and publish it. 
- ICorrelationContext is nothing more but the metadata that comes with the message
- Then our command or event handler will get this ICorrelationContext, to make it further will attach it in RabbitMQ publisher when your handler finishes the work.
### Handling error exception by IRejectedEvent
- Create a class that implement IRejectedEvent
- Create a seperate service that will subscribe to all such events. Once an event which might be either success or rejected, simply get the operation object from Database, let data access layer update its state so we would say that was a pending operation, now it's like rejected or successful.
- Within my `GenericEventHandler` we're listening down below to either rejected events or general event, so we know something failed or successful. 

