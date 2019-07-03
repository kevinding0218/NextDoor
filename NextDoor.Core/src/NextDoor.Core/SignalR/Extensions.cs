using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NextDoor.Core.Common;
using NextDoor.Core.src.NextDoor.Core.Redis;
using NextDoor.Core.Types;
using System;

namespace NextDoor.Core.SignalR
{
    public static class Extensions
    {
        public static void AddSignalr(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

            var options = configuration.GetOptions<SignalROptions>(ConfigOptions.signalrSectionName);
            services.AddSingleton(options);

            var builder = services.AddSignalR();
            if (!options.Backplane.Equals("redis", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var redisOptions = configuration.GetOptions<RedisOptions>(ConfigOptions.redisSectionName);
            builder.AddRedis(redisOptions.ConnectionString);
        }
    }
}
