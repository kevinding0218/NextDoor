using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.SqlSever
{
    public interface ISqlServerRepository<TEntity> where TEntity : IIdentifiable
    {
         Task<TEntity> GetSingleAsync(Guid id);
         Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
         Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
         Task AddAsync(TEntity entity);
         Task UpdateAsync(TEntity entity);
         Task DeleteAsync(Guid id);
         Task<bool> ExistedAsync(Expression<Func<TEntity, bool>> predicate);
         
         // Get List with pagination feature returned as IEnumerable
         Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase;
    }
}