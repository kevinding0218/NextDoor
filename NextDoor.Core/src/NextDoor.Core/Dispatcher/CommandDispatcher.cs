using System.Threading.Tasks;
using Autofac;
using NextDoor.Core.Handlers;
using NextDoor.Core.Messages;
using NextDoor.Core.RabbitMq;

namespace NextDoor.Core.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _context;
        public CommandDispatcher(IComponentContext context)
        {
            _context = context;
        }
        // Retrieve a service "ICommandHandler" from the context.
        public async Task SendAsync<T>(T command) where T : ICommand
            => await _context.Resolve<ICommandHandler<T>>().HandleAsync(command, CorrelationContext.Empty);
    }
}