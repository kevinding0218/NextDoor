using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Common;
using NextDoor.Core.MsSql;
using NextDoor.Core.MSSQL;
using NextDoor.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.MSSQL
{
    public static class Extensions
    {
        public static IServiceCollection AddEfMsSqlContext<msSqlContext>(this IServiceCollection service)
            where msSqlContext : DbContext
            => service.AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<msSqlContext>();

        public static void AddEfDatabase<myDbContext>(this ContainerBuilder builder, IServiceCollection services)
            where myDbContext : DbContext
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<MsSqlDbOptions>("mssql");

                return options;
            }).SingleInstance();

            services.AddEfMsSqlContext<myDbContext>();

            builder.RegisterType<MsSqlInitializer>()
                .As<IInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MsSqlDataSeeder<myDbContext>>()
                .As<IDataSeeder>()
                .InstancePerLifetimeScope();
        }
    }
}
