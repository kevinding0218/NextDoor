using Autofac;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Domain;

namespace NextDoor.Core.Mongo
{
    public static class Extensions
    {
        public static void AddMongo(this ContainerBuilder builder)
        {
            // Similar with AddSingleton
            // One instance is returned from all requests in the root and all nested scopes
            // Register a concrete type <MongoDbOptions> with lambda initiation
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<MongoDbOptions>(ConfigOptions.mongoSectionName);

                return options;
            }).SingleInstance();

            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbOptions>();

                return new MongoClient(options.ConnectionString);
            }).SingleInstance();

            // Similar with AddScoped
            builder.Register(context =>
            {
                var options = context.Resolve<MongoDbOptions>();
                var client = context.Resolve<MongoClient>();
                return client.GetDatabase(options.Database);
            }).InstancePerLifetimeScope();

            /* 
                    Register by Type - When using reflection-based components, 
                    Autofac automatically uses the constructor for your class with the most parameters 
                    that are able to be obtained from the container.
                    Any component type you register via RegisterType must be a concrete type. 
                    While components can expose abstract classes or interfaces as services, 
                    you can’t register an abstract/interface component.
            */
            builder.RegisterType<MongoDbInitializer>()
                .As<IMongoDbInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MongoDbSeeder>()
                .As<IDataSeeder>()
                .InstancePerLifetimeScope();
        }

        public static void AddMongoRepository<TEntity>(this ContainerBuilder builder, string collectionName)
            where TEntity : class, IGuidIdentifiable
            => builder.Register(ctx => new MongoRepository<TEntity>(ctx.Resolve<IMongoDatabase>(), collectionName))
                .As<IMongoRepository<TEntity>>()
                .InstancePerLifetimeScope();
    }
}