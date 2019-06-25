using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Common;
using NextDoor.Core.Types;

namespace NextDoor.Core.MSSQL
{
    public static class Extensions
    {
        public static IServiceCollection AddEntityFrameworkMsSql<myDbContext>(this IServiceCollection service)
            where myDbContext : DbContext
            => service.AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<myDbContext>();

        public static void AddEfDatabase<myDbContext>(this ContainerBuilder builder, IServiceCollection services)
            where myDbContext : DbContext
        {
            //builder.Register(context =>
            //{
            //    var configuration = context.Resolve<IConfiguration>();
            //    var options = configuration.GetOptions<MsSqlDbOptions>(ConfigOptions.mssqlSectionName);

            //    return options;
            //}).SingleInstance();

            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }
            var options = configuration.GetOptions<MsSqlDbOptions>(ConfigOptions.mssqlSectionName);
            services.AddSingleton(options);
            services.AddEntityFrameworkMsSql<myDbContext>();

            /*
             * For SeedData Injection
            builder.RegisterType<MsSqlInitializer>()
                .As<IInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MsSqlDataSeeder<myDbContext>>()
                .As<IDataSeeder>()
                .InstancePerLifetimeScope();
            */
        }
    }
}
