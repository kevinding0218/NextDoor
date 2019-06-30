using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextDoor.ApiGateway.Services;
using NextDoor.Core.Authentication;
using NextDoor.Core.Dispatcher;
using NextDoor.Core.Mvc;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.RestEase;
using NextDoor.Core.src.NextDoor.Core.Redis;
using Serilog;
using System;

namespace NextDoor.ApiGateway
{
    public class Startup
    {
        private static readonly string[] Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
        public IContainer Container { get; private set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc();

            #region Jwt&Redis
            services.AddJwt();
            services.AddRedis();
            #endregion

            #region Policy Based Authorization
            // Will be used by Core.Authentication.AdminAuthAttribute class 
            // and [AdminAuth] as Authorization Filter
            services.AddAuthorization(auth => auth.AddPolicy("admin", policy => policy.RequireRole("admin")));
            #endregion

            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", cors =>
                        cors.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithExposedHeaders(Headers));
            });
            #endregion

            #region RestEase
            services.RegisterServiceForwarder<IIdentityService>("identity-service");
            services.RegisterServiceForwarder<ICustomersService>("customer-service");
            services.RegisterServiceForwarder<IAdminService>("admin-service");
            services.RegisterServiceForwarder<INotificationService>("notification-service");
            #endregion

            return BuilderContainer(services);
        }

        #region Using Autofac Container
        private IServiceProvider BuilderContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            // Whenever registered by asp.net by default in this services object, move to our container builder
            builder.Populate(services);

            #region Register Dispatcher
            builder.AddDispatchers();
            #endregion

            #region RabbitMq
            builder.AddRabbitMq();
            #endregion

            Container = builder.Build();

            return new AutofacServiceProvider(Container);
        }
        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Middleware
            app.UseCors("CorsPolicy");
            app.UseAllForwardedHeader();
            app.UseErrorHandler();
            app.UseAuthentication();
            app.UseAccessTokenValidator();
            app.UseServiceId();
            app.UseHttpsRedirection();
            // logging
            loggerFactory.AddSerilog();
            app.UseMvc();
            app.UseRabbitMq();
            #endregion

            // be sure no application steps I will dispose my container
            // so if there will be any external connections or our files being opened
            // I wouldn't like them to be to get locked.
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());
        }
    }
}
