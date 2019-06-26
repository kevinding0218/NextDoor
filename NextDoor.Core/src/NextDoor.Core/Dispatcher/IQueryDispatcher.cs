using System.Threading.Tasks;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.Dispatcher
{
    public interface IQueryDispatcher
    {
         Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}