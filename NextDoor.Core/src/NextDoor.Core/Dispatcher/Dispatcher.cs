using NextDoor.Core.Messages;
using NextDoor.Core.Types.Pagination;
using System.Threading.Tasks;

namespace NextDoor.Core.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public Dispatcher(ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        // Call CommandDispatcher.SendAsync
        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
            => await _commandDispatcher.SendAsync(command);

        // Call QueryDispatcher.QueryAsync
        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
            => await _queryDispatcher.QueryAsync<TResult>(query);
    }
}