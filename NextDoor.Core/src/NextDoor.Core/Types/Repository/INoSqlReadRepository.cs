using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.Types.Repository
{
    public interface INoSqlReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IGuidIdentifiable
    {
        #region READ BY GUID
        Task<TEntity> GetSingleAsync(Guid guid);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
