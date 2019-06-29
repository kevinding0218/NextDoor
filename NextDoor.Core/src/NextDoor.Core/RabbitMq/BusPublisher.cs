using NextDoor.Core.Common;
using NextDoor.Core.Messages;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;
using System.Reflection;
using System.Threading.Tasks;

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

        /// <summary>
        /// Publish a TCommand through RabbitMQ
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command">TCommand Type</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task SendAsync<TCommand>(TCommand @command, ICorrelationContext context) where TCommand : ICommand
            => await _busClient.PublishAsync(@command, ctx => ctx.UseMessageContext(context)
                .UsePublishConfiguration(p => p.WithRoutingKey(GetRoutingKey(@command))));

        /// <summary>
        /// Publish a Event through RabbitMQ
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command">TCommand Type</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context)
            where TEvent : IEvent
            => await _busClient.PublishAsync(@event, ctx => ctx.UseMessageContext(context)
                .UsePublishConfiguration(p => p.WithRoutingKey(GetRoutingKey(@event))));

        /// <returns>Return Routing Key as Exchange.ICommand/IEvent(Class Name.Underscore()), e.g: #.identity.sign_up_cmd</returns>
        private string GetRoutingKey<T>(T message)
        {
            var ExchangeNamespace = message.GetType().GetCustomAttribute<ExchangeNamespaceAttribute>()?.Namespace ??
                             _defaultNamespace;
            ExchangeNamespace = string.IsNullOrWhiteSpace(ExchangeNamespace) ? string.Empty : $"{ExchangeNamespace}.";

            return $"{ExchangeNamespace}{typeof(T).Name.Underscore()}".ToLowerInvariant();
        }
    }
}