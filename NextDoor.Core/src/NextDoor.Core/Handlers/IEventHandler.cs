using System.Threading.Tasks;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;

namespace NextDoor.Core.Handlers
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, ICorrelationContext context);
    }
}