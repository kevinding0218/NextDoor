using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Services.Signalr.Messages.Events;
using NextDoor.Services.Signalr.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Signalr.Handlers
{
    public class OperationUpdatedHandler : IEventHandler<OperationPending>,
        IEventHandler<OperationCompleted>, IEventHandler<OperationRejected>
    {
        private readonly IHubService _hubService;

        public OperationUpdatedHandler(IHubService hubService)
        {
            _hubService = hubService;
        }

        public async Task HandleAsync(OperationPending @event, ICorrelationContext context)
            => await _hubService.PublishOperationPendingAsync(@event);

        public async Task HandleAsync(OperationCompleted @event, ICorrelationContext context)
            => await _hubService.PublishOperationCompletedAsync(@event);

        public async Task HandleAsync(OperationRejected @event, ICorrelationContext context)
            => await _hubService.PublishOperationRejectedAsync(@event);
    }
}
