using System;
using System.Threading.Tasks;

namespace NextDoor.Core.Types.Repository
{
    public interface IAsyncCUDRepository<TEntity> where TEntity : class
    {
        #region CREATE/UPDATE/DELETE
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid guid);
        #endregion
    }
}