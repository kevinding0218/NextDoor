using Microsoft.Extensions.Logging;
using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Services.Customers.Messages.Events;
using System.Threading.Tasks;

namespace NextDoor.Services.Customers.Handlers.Identity
{
    public class SignUpSuccessEventHandler : IEventHandler<SignUpSuccessEvent>
    {
        private readonly ILogger<SignUpSuccessEventHandler> _logger;

        public SignUpSuccessEventHandler(ILogger<SignUpSuccessEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(SignUpSuccessEvent @event, ICorrelationContext context)
        {
            _logger.LogInformation($"A new customer {@event.Email} has signed up!");

            return Task.CompletedTask;
        }
    }
}
