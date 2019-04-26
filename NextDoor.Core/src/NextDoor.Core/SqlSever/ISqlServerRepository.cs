using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.SqlSever
{
    public interface ISqlServerRepository<TEntity> where TEntity : class, IIdentifiable
    {
         #region CRUD
         Task<TEntity> GetSingleAsync(Guid id);
         Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
         Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
         void Add(TEntity entity);
         void Update(TEntity entity);
         void Delete(Guid id);
         #endregion

         #region Helper
         Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate);
         
         // TODO: Get List returned as IEnumerable which will implement pagination feature 
         Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase;
         #endregion
    }
}