using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Common;
using NextDoor.Core.Types;

namespace NextDoor.Core.src.NextDoor.Core.Redis
{
    public static class Extensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

            services.Configure<RedisOptions>(configuration.GetSection(ConfigOptions.redisMqSectionName));
            var options = configuration.GetOptions<RedisOptions>(ConfigOptions.redisMqSectionName);

            // setting up Redis distributed cache related services
            services.AddDistributedRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

            return services;
        }
    }
}
