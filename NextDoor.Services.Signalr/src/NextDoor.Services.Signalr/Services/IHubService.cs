using NextDoor.Services.Signalr.Messages.Events;
using System.Threading.Tasks;

namespace NextDoor.Services.Signalr.Services
{
    public interface IHubService
    {
        Task PublishOperationPendingAsync(OperationPending @event);
        Task PublishOperationCompletedAsync(OperationCompleted @event);
        Task PublishOperationRejectedAsync(OperationRejected @event);
    }
}
