﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Authentication;
using NextDoor.Core.Dispatcher;
using NextDoor.Core.Mongo;
using NextDoor.Core.MSSQL;
using NextDoor.Core.Mvc;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.src.NextDoor.Core.Redis;
using NextDoor.Core.Types;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Infrastructure.EF;
using System;
using System.Reflection;

namespace NextDoor.Services.Admin
{
    public class Startup
    {
        public IContainer Container { get; private set; }
        public IConfiguration Configuration { get; }

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

            Shared.UseSql = Convert.ToBoolean(Configuration["datasource:useSql"]);

            #region EF MsSql DbContext
            services.Configure<MsSqlDbOptions>(Configuration.GetSection(ConfigOptions.mssqlSectionName));
            services.AddEntityFrameworkMsSql<NextDoorDbContext>();
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

            return BuilderContainer(services);
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

            #region Mongo Db
            builder.AddMongo();
            // Create Mongodb collection based on class
            builder.AddMongoRepository<User>("Users");
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
            IApplicationLifetime applicationLifetime)
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
            app.UseHttpsRedirection();
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
