using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Core.Types.Repository
{
    public interface IAsyncCUDRepository<TEntity> where TEntity : class
    {
        #region CREATE/UPDATE/DELETE
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task DeleteAsync(Guid guid);
        Task DeleteRangeAsync(IEnumerable<Guid> guids);
        #endregion
    }
}