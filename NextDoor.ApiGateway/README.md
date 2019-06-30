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