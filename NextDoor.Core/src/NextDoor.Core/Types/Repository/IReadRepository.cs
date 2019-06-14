using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.Types.Repository
{
    public interface IReadRepository<TEntity> where TEntity : class
    {
        #region SHARED
        Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate);

        // TODO: Get List returned as IEnumerable which will implement pagination feature 
        Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase;
        #endregion
    }
}