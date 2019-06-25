using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Authentication;
using NextDoor.Core.Mongo;
using NextDoor.Core.MSSQL;
using NextDoor.Core.Mvc;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF;
using NextDoor.Services.Identity.Services.AutoMapper;
using NextDoor.Services.Identity.Services.Dto;
using System;

namespace NextDoor.Services.Identity
{
    public class Startup
    {
        // Autofac Ioc Container 
        public IContainer Container { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc();
            services.AddJwt();

            #region EF MsSql DbContext
            services.Configure<MsSqlDbOptions>(Configuration.GetSection(ConfigOptions.mssqlSectionName));
            services.AddEntityFrameworkMsSql<NextDoorDbContext>();
            #endregion

            #region Register AutoMapper Service
            services.AddAutoMapper(typeof(IdentityAutoMapperConfig));
            #endregion

            #region Using Autofac Container
            var builder = new ContainerBuilder();
            // Register all the interfaces along with its default implementations
            // within my assembly being Identity project
            //builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
            //    .AsImplementedInterfaces();
            builder.RegisterModule<InfrastructureModule>();
            // Whenever registered by asp.net by default in this services object, move to our container builder
            builder.Populate(services);
            builder.RegisterType<PasswordHasher<UserDto>>().As<IPasswordHasher<UserDto>>();
            builder.RegisterType<PasswordHasher<User>>().As<IPasswordHasher<User>>();

            builder.AddMongo();
            // Create Mongodb collection based on class
            builder.AddMongoRepository<RefreshToken>("RefreshTokens");
            builder.AddMongoRepository<User>("Users");
            #endregion

            Container = builder.Build();

            return new AutofacServiceProvider(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Inject IApplicationLifetime
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment() || env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseErrorHandler();
            app.UseHttpsRedirection();
            app.UseMvc();

            // be sure no application steps I will dispose my container
            // so if there will be any external connections or our files being opened
            // I wouldn't like them to be to get locked.
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());
        }
    }
}
