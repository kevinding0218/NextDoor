using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Authentication;
using NextDoor.Core.Common;
using NextDoor.Core.Dispatcher;
using NextDoor.Core.Mvc;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.src.NextDoor.Core.Redis;
using NextDoor.Core.Types;
using NextDoor.Services.Signalr.Framework;
using NextDoor.Services.Signalr.Hubs;
using NextDoor.Services.Signalr.Messages.Events;
using System;
using System.Reflection;

namespace NextDoor.Services.Signalr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }

        public Startup(IConfiguration configuration)
        {
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

            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", cors =>
                        cors.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
            });
            #endregion
            AddSignalR(services);

            return BuilderContainer(services);
        }

        private void AddSignalR(IServiceCollection services)
        {
            var options = Configuration.GetOptions<SignalrOptions>(ConfigOptions.signalrSectionName);
            services.AddSingleton(options);
            var builder = services.AddSignalR();
            if (!options.Backplane.Equals("redis", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            var redisOptions = Configuration.GetOptions<RedisOptions>(ConfigOptions.redisSectionName);
            builder.AddRedis(redisOptions.ConnectionString);
        }

        #region Using Autofac Container
        private IServiceProvider BuilderContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            // Register all the interfaces along with its default implementations
            // within my assembly being Identity project
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                .AsImplementedInterfaces();

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
            IApplicationLifetime applicationLifetime, SignalrOptions signalrOptions,
            IStartupInitializer startupInitializer)
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
            app.UseStaticFiles();
            app.UseErrorHandler();
            app.UseAuthentication();
            app.UseAccessTokenValidator();
            app.UseServiceId();
            app.UseHttpsRedirection();
            app.UseSignalR(routes =>
            {
                routes.MapHub<NextDoorHub>($"/{signalrOptions.Hub}");
            });
            app.UseMvc();
            app.UseRabbitMq()
                .SubscribeEvent<OperationPending>(@namespace: "signalr")
                .SubscribeEvent<OperationCompleted>(@namespace: "signalr")
                .SubscribeEvent<OperationRejected>(@namespace: "signalr"); ;
            #endregion

            startupInitializer.InitializeAsync();

            // be sure no application steps I will dispose my container
            // so if there will be any external connections or our files being opened
            // I wouldn't like them to be to get locked.
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());
        }
    }
}
