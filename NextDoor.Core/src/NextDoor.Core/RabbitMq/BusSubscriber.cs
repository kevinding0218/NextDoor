using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextDoor.Core.Common;
using NextDoor.Core.Handlers;
using NextDoor.Core.Messages;
using NextDoor.Core.Types;
using OpenTracing;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NextDoor.Core.RabbitMq
{
    public class BusSubscriber : IBusSubscriber
    {
        private readonly ILogger _logger;
        private readonly IBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;
        // OpenTracing: for span creation and propagation across arbitrary transports.
        private readonly ITracer _tracer;
        private readonly string _defaultNamespace;
        private readonly int _retries;
        private readonly int _retryInterval;

        public BusSubscriber(IApplicationBuilder app)
        {
            // IServiceProvider.GetService(): retrieve an instance of an object within IServiceCollection in the DI Container
            _logger = app.ApplicationServices.GetService<ILogger<BusSubscriber>>();
            _serviceProvider = app.ApplicationServices.GetService<IServiceProvider>();
            _busClient = _serviceProvider.GetService<IBusClient>();
            _tracer = _serviceProvider.GetService<ITracer>();
            var options = _serviceProvider.GetService<RabbitMqOptions>();
            _defaultNamespace = options.Namespace;
            _retries = options.Retries >= 0 ? options.Retries : 3;
            _retryInterval = options.RetryInterval > 0 ? options.RetryInterval : 2;
        }

        // Based on the message type/command, create a new subscription
        // Usage: busSubscriber.SubscribeCommand<UserCreated>();
        // _busClient from RawRabbit 
        public IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
            Func<TCommand, NextDoorException, IRejectedEvent> onError = null) where TCommand : ICommand
        {
            // Whenver this message comes through my service bus, will do (command, correlationContext)
            // use my service provider acting as a service locator
            // look for the command handler "ICommandHandler" that is able to handle this command "TCommand" and try to handle it
            // similar like what we have in our CommandDispathcer, 
            // but right now our command comes from the queue not from our memory or within the same process where our app lives
            this._busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
                {
                    var commandHandler = this._serviceProvider.GetService<ICommandHandler<TCommand>>();

                    return await TryHandleAsync(command, correlationContext,
                            () => commandHandler.HandleAsync(command, correlationContext), onError);
                },
                ctx => ctx.UseSubscribeConfiguration(cfg =>
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>(@namespace, queueName)))));

            return this;
        }

        // Based on the message type/event, create a new subscription
        // Usage: busSubscriber.SubscribeEvent<UserCreated>();
        public IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
            Func<TEvent, NextDoorException, IRejectedEvent> onError = null) where TEvent : IEvent
        {
            this._busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, correlationContext) =>
                {
                    var eventHandler = this._serviceProvider.GetService<IEventHandler<TEvent>>();

                    return await TryHandleAsync(@event, correlationContext,
                        () => eventHandler.HandleAsync(@event, correlationContext), onError);
                },
                ctx => ctx.UseSubscribeConfiguration(cfg =>
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>(@namespace, queueName)))));

            return this;
        }

        /* 
        * Get full command path from top level
        * Get the unique queues per instance of a different application that subscribes to the same event.
        * Not just return a typeof(T).Name because on the regular server this pattern works just fine, 
          but once you put .NET Core application into Docker container it will get the same Assembly name as the other applications 
          (e.g. “app” depending on the WORKDIR)
        */
        private string GetQueueName<T>(string @namespace = null, string name = null)
        {
            @namespace = string.IsNullOrWhiteSpace(@namespace)
                ? (string.IsNullOrWhiteSpace(_defaultNamespace) ? string.Empty : _defaultNamespace)
                : @namespace;

            var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

            return (string.IsNullOrWhiteSpace(name)
                ? $"{Assembly.GetEntryAssembly().GetName().Name}/{separatedNamespace}{typeof(T).Name.Underscore()}"
                : $"{name}/{separatedNamespace}{typeof(T).Name.Underscore()}").ToLowerInvariant();
        }

        // Internal retry for services that subscribe to the multiple events of the same types.
        // It does not interfere with the routing keys and wildcards (see TryHandleWithRequeuingAsync() below).
        private async Task<Acknowledgement> TryHandleAsync<TMessage>(TMessage message,
            CorrelationContext correlationContext,
            Func<Task> handle, Func<TMessage, NextDoorException, IRejectedEvent> onError = null)
        {
            await handle();
            return new Ack();
            //var currentRetry = 0;
            //var retryPolicy = Policy
            //    .Handle<Exception>()
            //    .WaitAndRetryAsync(this._retries, i => TimeSpan.FromSeconds(this._retryInterval));

            //var messageName = message.GetType().Name;

            //return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
            //{
            //    var scope = this._tracer
            //        .BuildSpan("executing-handler")
            //        .AsChildOf(this._tracer.ActiveSpan)
            //        .StartActive(true);

            //    using (scope)
            //    {
            //        var span = scope.Span;

            //        try
            //        {
            //            var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}'.";

            //            var preLogMessage = $"Handling a message: '{messageName}' " +
            //                                $"with correlation id: '{correlationContext.Id}'. {retryMessage}";

            //            this._logger.LogInformation(preLogMessage);
            //            span.Log(preLogMessage);

            //            await handle();

            //            var postLogMessage = $"Handled a message: '{messageName}' " +
            //                                 $"with correlation id: '{correlationContext.Id}'. {retryMessage}";
            //            this._logger.LogInformation(postLogMessage);
            //            span.Log(postLogMessage);

            //            return new Ack();
            //        }
            //        catch (Exception exception)
            //        {
            //            currentRetry++;
            //            this._logger.LogError(exception, exception.Message);
            //            span.Log(exception.Message);
            //            span.SetTag(Tags.Error, true);

            //            if (exception is NextDoorException nextDoorException && onError != null)
            //            {
            //                var rejectedEvent = onError(message, nextDoorException);
            //                await this._busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));
            //                this._logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
            //                                            $"for the message: '{messageName}' with correlation id: '{correlationContext.Id}'.");
            //                span.SetTag("error-type", "domain");
            //                return new Ack();
            //            }

            //            span.SetTag("error-type", "infrastructure");
            //            throw new Exception($"Unable to handle a message: '{messageName}' " +
            //                                $"with correlation id: '{correlationContext.Id}', " +
            //                                $"retry {currentRetry - 1}/{this._retries}...");
            //        }
            //    }
            //});
        }

        // RabbitMQ retry that will publish a message to the retry queue.
        // Keep in mind that it might get processed by the other services using the same routing key and wildcards.
        private async Task<Acknowledgement> TryHandleWithRequeuingAsync<TMessage>(TMessage message,
            CorrelationContext correlationContext,
            Func<Task> handle, Func<TMessage, NextDoorException, IRejectedEvent> onError = null)
        {
            var messageName = message.GetType().Name;
            var retryMessage = correlationContext.Retries == 0
                ? string.Empty
                : $"Retry: {correlationContext.Retries}'.";
            _logger.LogInformation($"Handling a message: '{messageName}' " +
                                   $"with correlation id: '{correlationContext.Id}'. {retryMessage}");

            try
            {
                await handle();
                _logger.LogInformation($"Handled a message: '{messageName}' " +
                                       $"with correlation id: '{correlationContext.Id}'. {retryMessage}");

                return new Ack();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                if (exception is NextDoorException dShopException && onError != null)
                {
                    var rejectedEvent = onError(message, dShopException);
                    await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));
                    _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                           $"for the message: '{messageName}' with correlation id: '{correlationContext.Id}'.");

                    return new Ack();
                }

                if (correlationContext.Retries >= _retries)
                {
                    await _busClient.PublishAsync(RejectedEvent.For(messageName),
                        ctx => ctx.UseMessageContext(correlationContext));

                    throw new Exception($"Unable to handle a message: '{messageName}' " +
                                        $"with correlation id: '{correlationContext.Id}' " +
                                        $"after {correlationContext.Retries} retries.", exception);
                }

                _logger.LogInformation($"Unable to handle a message: '{messageName}' " +
                                       $"with correlation id: '{correlationContext.Id}', " +
                                       $"retry {correlationContext.Retries}/{_retries}...");

                return Retry.In(TimeSpan.FromSeconds(_retryInterval));
            }
        }
    }
}