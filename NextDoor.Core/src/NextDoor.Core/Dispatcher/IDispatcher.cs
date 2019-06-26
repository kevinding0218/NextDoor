using System.Threading.Tasks;
using NextDoor.Core.Messages;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.Dispatcher
{
    public interface IDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}