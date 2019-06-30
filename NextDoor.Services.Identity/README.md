## JsonConstructor
- Let system know how to handle serialization for your constructor
## Multiple ways of Sign Up
- Using traditional Service Layer and Dto and DI in Controller
- Using local dispatch
	- Create a Command Type class of `SignUpSubscribeCmd` which inherites from `ICommand`, the `SignUpSubscribeCmd` would be used as our Dto model
	- Create a Command Handler class of `SignUpSubscribeCmdHandler` which inherites from `ICommandHandler<SignUpCmd>`, this would be our local handler to use Repository to perform business logic.
	- In `IdentityController`, use `_dispatcher.SendAsync(command.BindId(c => c.CommandId));` to send incoming request/command of `SignUpSubscribeCmd`, 
	- Inside the `CommandDispatcher.SendAsync` method, will automatically look for any Command Handler for the corresponding type of `ICommand`, here is our `ICommandHandler<SignUpCmd>` which resolve the incoming request in `HandleAsync` and consume it.
- Using Api Gateway and RabbitMQ
	- Create a new Api Gateway Service, inside of its controller action, call `await SendAsync(command.BindId(c => c.CommandId), resourceId: command.CommandId, resource: "identity");` to send the command in RabbitMQ with Routing key formatted as **Exchange.ICommand/IEvent(Class Name.Underscore())**, e.g: `#.identity.sign_up_cmd`
	- Right now the message which also contains the `SignUpCmd` model has been sent to RabbitMQ with defined Routing Key, we need to register the consumer to handle this published event.
	- Create a Subscriber with same Command Name as middleware in `IdentityService`, the `SubscribeCommand` method will use the same `ICommandHandler<SignUpCmd>` to handle this command
	```
	app.UseRabbitMq()
		.SubscribeCommand<SignUpCmd>();
	```
## Multiple ways of Sign In
- Using traditional Service Layer and Dto and DI in Controller
- Using local dispatch
	- Since Sign In requires to return something as in response such like JwtToken, we need to use query and query handler to perform this.
	- Create a Query Type class of `SignInQuery` which inherites from `IQuery<JsonWebToken>`, the `SignInQuery` would be used as our Dto model
	- Create a Query Handler class of `SignInQueryHandler` which inherites from `IQueryHandler<SignInQuery, JsonWebToken>`, this would be our local handler to use Repository to perform business logic. 
	- In `IdentityController`, use `await _dispatcher.QueryAsync(query);` to send incoming request/query of `SignInQuery`, 
	- Inside the `QueryDispatcher.QueryAsync` method, will automatically look for any Query Handler for the corresponding type of `IQuery<TResult>`, here is our `IQueryHandler<SignInQuery, JsonWebToken>` which resolve the incoming request in `HandleAsync` and consume it.
- Using Api Gateway and RestEase
	- Create a new Api Gateway Service, also create a `IIdentityService` interface which will be used as our RestEase redirection, define the way of Http Action and return type as same as what we will be used later on in our internal service.
	```
	[AllowAnyStatusCode]
    [Post("sign-in")]
    Task<JsonWebToken> SignInAsync([Query] SignInQuery query);
	```
	- RestEase will look for the appsettings.json config for where the service name mapping to which host and port, then in StartUp class, register the `IIdentityService` with the config we defined `restEase.services.identity-service`
	```
	services.RegisterServiceForwarder<IIdentityService>("identity-service");
	```
	- Inside of its controller action, define a action method by injecting and using `IIdentityService`, whenever a http call reaches our API gateway, it will then redirect to our IdentityService http post method of sign-in.
	```
	[HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> SignIn(SignInQuery query)
        //=> Single(await _identityService.SignInAsync(query));
        => await _identityService.SignInAsync(query);
	```
	- Then same approach of local dispatch will be executed within identity.service and return our JwtToken.
## RabbitMQ
- we subscribe to particular message when the app actually starts,
the app subscribes on the very beginning of its lifetime, the subscription leaves together with the particular micro service
- when app starts, we can see there is a "TOPIC" exchange of "Identity" in RabbitMQ Exchange.
- one application will be publishing messages, it can be our API publishing commands or it can be our services publishing other commands or events, and this command goes to the RabbitMQ, so called exchange. And then from this exchange depending on the queue that is its interested in this particular message type which is defined by the routing key, it just creates a queue and this messages that are currently leaving these actions are being published and pushed into the queues.
- the default setup for the routing key in a rar rabbit is the message as assembly, that actually had to be changd in order to support messages that lives in two seperated assemblies.
	- Right now, our "SignUpCmd" lives in the namespace dedicated to the identity service, and because we're not going to have any packages with our public contracts(meaning we're not writing an external contract that involving all command/event/query across all services)
	- each micro service should only contain the messages that contained in the matter of the domain of the particular micro service.
	- routing key by default: **namespace in appsettings  + command/event/query name** 
		- e.g my routing key will be look like **#.identity.sign_up_cmd**
	- routing key by customize: use ["MessageNamespace("anotherServiceNamespace")]: if I'm interested in a particular message that's not in my service which would publish it but another service that wants to subscribe to this type of message, use the annotation and set it as its routing key. (See Core.RabbitMq.Extensions.CustomNamingConventions)
	- add same message to api gateway which will have different namespace, in order to get rid of the namespace routing key to make it work, we'd customized the namespace either in configuration or attribute of `MessageNamespace`
