using System;
using NextDoor.Core.Messages;
using NextDoor.Core.Types;

namespace NextDoor.Core.RabbitMq
{
    public interface IBusSubscriber
    {
        IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
            Func<TCommand, NextDoorException, IRejectedEvent> onError = null) where TCommand : ICommand;
        IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
            Func<TEvent, NextDoorException, IRejectedEvent> onError = null) where TEvent : IEvent;
    }
}