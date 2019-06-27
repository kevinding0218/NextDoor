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
        // Whenver this message comes through in-memory
        // will look for the command handler "ICommandHandler" that is able to handle this command "TCommand" and try to handle it
        // our command comes from our memory or within the same process where our app lives
        public async Task SendAsync<T>(T command) where T : ICommand
            => await _context.Resolve<ICommandHandler<T>>().HandleAsync(command, CorrelationContext.Empty);
    }
}