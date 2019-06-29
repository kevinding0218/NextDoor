using Microsoft.Extensions.Logging;
using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Services.Notifications.Messages.Events;
using System.Threading.Tasks;

namespace NextDoor.Services.Notifications.Handlers.Identity
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
            // Send email for verification process

            return Task.CompletedTask;
        }
    }
}
