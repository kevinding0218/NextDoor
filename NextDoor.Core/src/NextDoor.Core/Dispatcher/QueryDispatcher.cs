using Autofac;
using NextDoor.Core.Handlers;
using NextDoor.Core.Types.Pagination;
using System.Threading.Tasks;

namespace NextDoor.Core.Dispatcher
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IComponentContext _context;

        public QueryDispatcher(IComponentContext context)
        {
            _context = context;
        }

        // Whenver IQuery sent through in-memory/local dispatcher
        // will look for the command handler "IQueryHandler" that is able to handle this command "IQuery" 
        // our command comes from our memory or within the same process where our app lives
        public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _context.Resolve(handlerType);

            return await handler.HandleAsync((dynamic)query);
        }
    }
}