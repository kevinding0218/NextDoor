using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
using NextDoor.Services.Identity.Infrastructure;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF;
using NextDoor.Services.Identity.Messages.Commands;
using NextDoor.Services.Identity.Messages.Events;
using NextDoor.Services.Identity.Services.AutoMapper;
using NextDoor.Services.Identity.Services.Dto;
using System;

namespace NextDoor.Services.Identity
{
    public class Startup
    {
        private static readonly string[] Headers = new[] { "X-Operation", "X-Resource", "X-Total-Count" };
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
            // services.AddConsul();

            #region Jwt&Redis
            services.AddJwt();
            services.AddRedis();
            #endregion

            Shared.UseSql = Convert.ToBoolean(Configuration["datasource:useSql"]);

            #region EF MsSql DbContext
            services.Configure<MsSqlDbOptions>(Configuration.GetSection(ConfigOptions.mssqlSectionName));
            services.AddEntityFrameworkMsSql<NextDoorDbContext>();
            #endregion

            #region Seed Mongo
            // services.AddInitializers(typeof(IMongoDbInitializer));
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(IdentityAutoMapperConfig));
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

            return BuilderContainer(services);
        }

        #region Using Autofac Container
        private IServiceProvider BuilderContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            #region Register all the interfaces along with its default implementations within my assembly being Identity project
            builder.RegisterModule<InfrastructureModule>();
            //builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
            //    .AsImplementedInterfaces();
            #endregion

            // Whenever registered by asp.net by default in this services object, move to our container builder
            builder.Populate(services);
            #region Password Hasher
            builder.RegisterType<PasswordHasher<UserDto>>().As<IPasswordHasher<UserDto>>();
            builder.RegisterType<PasswordHasher<User>>().As<IPasswordHasher<User>>();
            #endregion

            #region Register Dispatcher
            builder.AddDispatchers();
            #endregion

            #region Mongo Db
            builder.AddMongo();
            // Create Mongodb collection based on class
            builder.AddMongoRepository<RefreshToken>("RefreshTokens");
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
        // Inject IApplicationLifetime
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, IConsulClient consulClient)
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

            #region Middleware
            app.UseCors("CorsPolicy");
            app.UseAllForwardedHeader();
            app.UseErrorHandler();
            app.UseAuthentication();
            app.UseAccessTokenValidator();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseRabbitMq()
                .SubscribeCommand<SignUpCmd>(onError: (cmd, ex)
                    => new SignUpRejectedEvent(cmd.Email, ex.Message, "signup_failed"));
            // var serviceId = app.UseConsul();
            #endregion

            // be sure no application steps I will dispose my container
            // so if there will be any external connections or our files being opened
            // I wouldn't like them to be to get locked.
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                // if our application becomes offline, it will remove itself from the consul
                // consulClient.Agent.ServiceDeregister(serviceId);
                Container.Dispose();
            });
        }
    }
}
