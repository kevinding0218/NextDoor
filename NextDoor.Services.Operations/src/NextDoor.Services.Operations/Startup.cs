using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Dispatcher;
using NextDoor.Core.Mvc;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.src.NextDoor.Core.Redis;
using System;
using System.Reflection;

namespace NextDoor.Services.Operations
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

            services.AddRedis();

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

            return BuilderContainer(services);
        }

        #region Using Autofac Container
        private IServiceProvider BuilderContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseHttpsRedirection();
            app.UseErrorHandler();
            app.UseServiceId();
            app.UseMvc();
            app.UseRabbitMq()
                .SubscribeAllMessages();
            #endregion
        }
    }
}
