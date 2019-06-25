using NextDoor.Core.Types.Pagination;
using System.Threading.Tasks;

namespace NextDoor.Core.Handlers
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
