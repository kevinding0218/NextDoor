using System.Threading;
using System;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using NextDoor.Core.Common;
using NextDoor.Core.Messages;
using OpenTracing;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using RawRabbit.Pipe.Middleware;
using System.Threading.Tasks;
using RawRabbit.Pipe;
using RawRabbit.Enrichers.MessageContext;
using Microsoft.Extensions.Configuration;
using NextDoor.Core.Handlers;

namespace NextDoor.Core.RabbitMq
{
    public static class Extensions
    {
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new BusSubscriber(app);

        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            // Similar with AddSingleton
            // Register from customed <RabbitMqOptions>
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<RabbitMqOptions>("rabbitMq");

                return options;
            }).SingleInstance();

            // Register from <RawRabbitConfiguration>
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<RawRabbitConfiguration>("rabbitMq");

                return options;
            }).SingleInstance();

            var assembly = Assembly.GetCallingAssembly();

            // Similar with AddTransient()
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .InstancePerDependency();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerDependency();
            // builder.Register<Handler>().As<IHandler>()
            //     .InstancePerDependency();
            builder.RegisterType<BusPublisher>().As<IBusPublisher>()
                .InstancePerDependency();
            // builder.RegisterInstance(DShopDefaultTracer.Create()).As<ITracer>().SingleInstance()
            //     .PreserveExistingDefaults();

            ConfigureBus(builder);
        }

        private static void ConfigureBus(ContainerBuilder builder)
        {
            builder.Register<IInstanceFactory>(context => 
            {
                var options = context.Resolve<RabbitMqOptions>();
                var configuration = context.Resolve<RawRabbitConfiguration>();
                var namingConventions = new CustomNamingConventions(options.Namespace);
                var tracer = context.Resolve<ITracer>();

                return RawRabbitFactory.CreateInstanceFactory(
                    new RawRabbitOptions
                    {
                        DependencyInjection = ioc =>
                        {
                            ioc.AddSingleton(options);
                            ioc.AddSingleton(configuration);
                            ioc.AddSingleton<INamingConventions>(namingConventions);
                            ioc.AddSingleton(tracer);
                        },
                        Plugins = p => p
                            .UseAttributeRouting()
                            .UseRetryLater()
                            .UpdateRetryInfo()
                            .UseMessageContext<CorrelationContext>()
                            .UseContextForwarding()
                            // .UseJaeger(tracer)
                    }
                );
            }).SingleInstance();
        }

        private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
        {
            clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

            return clientBuilder;
        }

        // Using "Nested Private" class for implementing third-party interfaces in a controlled environment where we can still access private members.
        #region nested private class "CustomNamingConventions"
        private class CustomNamingConventions : NamingConventions
        {
            public CustomNamingConventions(string defaultNamespace)
            {
                ExchangeNamingConvention = (Type type) => GetNamespace(type, defaultNamespace).ToLowerInvariant();
                RoutingKeyConvention = (Type type) => 
                    $"#.{GetRoutingKeyNamespace(type, defaultNamespace)}{type.Name.Underscore()}".ToLowerInvariant();
                ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
                RetryLaterExchangeConvention = (TimeSpan span) => $"{defaultNamespace}.retry";
                RetryLaterQueueNameConvetion = (string exchange, TimeSpan span) => 
                    $"{defaultNamespace}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms".ToLowerInvariant();
            }


            // Using "private static" so that the compiler will emit non-virtual call sites to these members. 
            // Emitting non-virtual call sites will prevent a check at runtime for each call that ensures that the current object pointer is non-null. 
            // This can result in a measurable performance gain for performance-sensitive code.
            private static string GetNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? "#" : $"{@namespace}";
            }

            private static string GetRoutingKeyNamespace(Type type, string defaultNamespace)
            {
                var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

                return string.IsNullOrWhiteSpace(@namespace) ? "#" : $"{@namespace}";
            }
        }
        #endregion

        // Using "Nested Private" class for implementing third-party interfaces in a controlled environment where we can still access private members.
        #region nested private class "RetryStagedMiddleware"
        private class RetryStagedMiddleware : StagedMiddleware
        {
            public override string StageMarker { get; } = RawRabbit.Pipe.StageMarker.MessageDeserialized;

            public override async Task InvokeAsync(IPipeContext context,
                CancellationToken token = new CancellationToken())
            {
                 var retry = context.GetRetryInformation();
                 if (context.GetMessageContext() is CorrelationContext message)
                 {
                     message.Retries = retry.NumberOfRetries;
                 }   

                 await Next.InvokeAsync(context, token);
            }
        }
        #endregion
    }
}