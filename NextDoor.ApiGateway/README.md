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
