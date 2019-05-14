using System.Reflection;
using System.Threading.Tasks;
using NextDoor.Core.Common;
using NextDoor.Core.Messages;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace NextDoor.Core.RabbitMq
{
    public class BusPublisher : IBusPublisher
    {
        private readonly IBusClient _busClient;
        private readonly string _defaultNamespace;

        public BusPublisher(IBusClient busClient, RabbitMqOptions options)
        {
            this._busClient = busClient;
            this._defaultNamespace = options.Namespace;
        }

        public async Task SendAsync<TCommand>(TCommand @command, ICorrelationContext context) where TCommand : ICommand
            => await _busClient.PublishAsync(@command, ctx => ctx.UseMessageContext(context)
                .UsePublishConfiguration(p => p.WithRoutingKey(GetRoutingKey(@command))));

        public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context)
            where TEvent : IEvent
            => await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context)
                .UsePublishConfiguration(p => p.WithRoutingKey(GetRoutingKey(@event))));

        private string GetRoutingKey<T>(T message)
        {
            var @namespace = message.GetType().GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? _defaultNamespace;
            @namespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

            return $"{@namespace}{typeof(T).Name.Underscore()}".ToLowerInvariant();
        }
    }
}