using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using NextDoor.Core.Types.ChangeLog;
using NextDoor.Core.Types.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

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

        /// <summary>
        /// Executes the specified raw SQL command and returns an integer specifying the number of rows affected by the SQL statement passed to it
        /// </summary>
        /// <param name="sql">The raw SQL e.g: EXEC AddCategory @CategoryName/"INSERT Categories (CategoryName) VALUES (@CategoryName)"</param>
        /// <param name="parameters">The SqlParameter.</param>
        /// <returns>The number of state entities written to database.</returns>
        public static int ExecuteSqlCommand(this DbContext _dbContext, string sql, params SqlParameter[] parameters) => _dbContext.Database.ExecuteSqlCommand(sql, parameters);

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{T}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        public static IQueryable<T> FromSql<T>(this DbContext _dbContext, string sql, params SqlParameter[] parameters) where T : class
            => _dbContext.Set<T>().FromSql(sql, parameters);

        public static IEnumerable<ChangeLog> GetChanges(this DbContext dbContext, IUserInfo userInfo)
        {
            var exclusions = dbContext.Set<ChangeLogExclusion>().ToList();

            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Modified)
                    continue;

                var entityType = entry.Entity.GetType();

                if (exclusions != null && (exclusions.Count(item => item.EntityName == entityType.Name && item.PropertyName == "*") == 1))
                    yield break;

                foreach (var property in entityType.GetTypeInfo().DeclaredProperties)
                {
                    // Validate if there is an exclusion for *.Property
                    if (exclusions != null && (exclusions.Count(item => item.EntityName == "*" && string.Compare(item.PropertyName, property.Name, true) == 0) == 1))
                        continue;

                    // Validate if there is an exclusion for Entity.Property
                    if (exclusions != null && (exclusions.Count(item => item.EntityName == entityType.Name && string.Compare(item.PropertyName, property.Name, true) == 0)) == 1)
                        continue;

                    var originalValue = entry.Property(property.Name).OriginalValue;
                    var currentValue = entry.Property(property.Name).CurrentValue;

                    if (string.Concat(originalValue) == string.Concat(currentValue))
                        continue;

                    // todo: improve the way to retrieve primary key value from entity instance

                    var key = entry.Entity.GetType().GetProperties().First().GetValue(entry.Entity, null).ToString();

                    yield return new ChangeLog
                    {
                        ClassName = entityType.Name,
                        PropertyName = property.Name,
                        Key = key,
                        OriginalValue = originalValue == null ? string.Empty : originalValue.ToString(),
                        CurrentValue = currentValue == null ? string.Empty : currentValue.ToString(),
                        Uid = userInfo.UID,
                        ChangeDate = DateTime.Now
                    };
                }
            }
        }
    }
}
