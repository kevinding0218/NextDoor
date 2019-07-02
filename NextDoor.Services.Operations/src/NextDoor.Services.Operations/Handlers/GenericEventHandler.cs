using Chronicle;
using NextDoor.Core.Handlers;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;
using NextDoor.Services.Operations.Sagas;
using NextDoor.Services.Operations.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Operations.Handlers
{
    public class GenericEventHandler<T> : IEventHandler<T> where T : class, IEvent
    {
        private readonly ISagaCoordinator _sagaCoordinator;
        private readonly IOperationPublisher _operationPublisher;
        private readonly IOperationsStorage _operationsStorage;

        public GenericEventHandler(ISagaCoordinator sagaCoordinator,
            IOperationPublisher operationPublisher,
            IOperationsStorage operationsStorage)
        {
            _sagaCoordinator = sagaCoordinator;
            _operationPublisher = operationPublisher;
            _operationsStorage = operationsStorage;
        }

        public async Task HandleAsync(T @event, ICorrelationContext context)
        {
            if (@event.BelongsToSaga())
            {
                var sagaContext = SagaContext.FromCorrelationContext(context);
                await _sagaCoordinator.ProcessAsync(@event, sagaContext);
            }

            switch (@event)
            {
                case IRejectedEvent rejectedEvent:
                    await _operationsStorage.SetAsync(context.Id, context.UserId,
                        context.Name, OperationState.Rejected, context.Resource,
                        rejectedEvent.Code, rejectedEvent.Reason);
                    await _operationPublisher.RejectAsync(context,
                        rejectedEvent.Code, rejectedEvent.Reason);
                    return;
                case IEvent _:
                    await _operationsStorage.SetAsync(context.Id, context.UserId,
                        context.Name, OperationState.Completed, context.Resource);
                    await _operationPublisher.CompleteAsync(context);
                    return;
            }
        }
    }
}
