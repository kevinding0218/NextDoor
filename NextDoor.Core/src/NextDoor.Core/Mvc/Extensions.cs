using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NextDoor.Core.Common;

namespace NextDoor.Core.Mvc
{
    public static class Extensions
    {
        #region MVC IServiceCollection
        public static IMvcCoreBuilder AddCustomMvc(this IServiceCollection services)
        {
            using(var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<AppOptions>(configuration.GetSection("app"));
            }

            // AddSingleton - creates a single instance throughout the application. 
            // It creates the instance for the first time and reuses the same object in the all calls.
            services.AddSingleton<IServiceId, ServiceId>();
            // AddTransient - created each time they are requested, a new instance is provided to every controller and every service.
            // This lifetime works best for lightweight, stateless services., 
            services.AddTransient<IStartupInitializer, StartupInitializer>();
            // In order to access HttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // AddScoped - same within a request, but different across different requests
            // lifetime services are created once per request within the scope. It is equivalent to Singleton in the current scope. 
            // eg. in MVC it creates 1 instance per each http request but uses the same instance in the other calls within the same web request.


            return services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddDataAnnotations()
                .AddApiExplorer()
                .AddAuthorization()
                .AddDefaultJsonOptions();
        }

        public static IServiceCollection AddInitializers(this IServiceCollection services, params Type[] initializers)
            => initializers == null
                ? services
                : services.AddTransient<IStartupInitializer, StartupInitializer>(c => {
                    var startupInitializer = new StartupInitializer();
                    var validInitializers = initializers.Where(t => typeof(IInitializer).IsAssignableFrom(t));
                    foreach(var initializer in validInitializers)
                    {
                        startupInitializer.AddInitializer(c.GetService(initializer) as IInitializer);
                    }

                    return startupInitializer;
                });
        #endregion

        #region IMvcCoreBuilder
        public static IMvcCoreBuilder AddDefaultJsonOptions(this IMvcCoreBuilder builder)
            => builder.AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                o.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                o.SerializerSettings.Formatting = Formatting.Indented;
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        #endregion

        #region IApplicationBuilder
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ErrorHandlerMiddleware>();
        
        /*
        Why use forwarded headers?
        In the recommended configuration for ASP.NET Core, the app is hosted using IIS/ASP.NET Core Module, Nginx, or Apache. Proxy servers, load balancers, 
        and other network appliances often obscure information about the request before it reaches the app:

        * When HTTPS requests are proxied over HTTP, the original scheme (HTTPS) is lost and must be forwarded in a header.
        * Because an app receives a request from the proxy and not its true source on the Internet or corporate network, 
        the originating client IP address must also be forwarded in a header.
        https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-2.2
         */
        public static IApplicationBuilder UseAllForwardedHeader(this IApplicationBuilder builder)
            => builder.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

        public static IApplicationBuilder UseServiceId(this IApplicationBuilder builder)
            => builder.Map("/id", c => c.Run(
                async ctx => {
                    using (var scope = c.ApplicationServices.CreateScope())
                    {
                        var id = scope.ServiceProvider.GetService<IServiceId>().Id;
                        await ctx.Response.WriteAsync(id);
                    }
                }
            ));
        #endregion
    }
}