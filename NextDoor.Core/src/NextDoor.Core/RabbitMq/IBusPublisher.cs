using System.Threading.Tasks;
using NextDoor.Core.Messages;

namespace NextDoor.Core.RabbitMq
{
    public interface IBusPublisher
    {
         Task SendAsync<TCommand>(TCommand @command, ICorrelationContext context) where TCommand : ICommand;
         // used reserved words as variable names
         Task PublishAsync<TEvent>(TEvent @event, ICorrelationContext context) where TEvent : IEvent;
    }
}