using System;
using System.Threading.Tasks;

namespace NextDoor.Core.Types
{
    public interface IAsyncCUDRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IIdentifiable
    {
         #region CREATE/UPDATE/DELETE
         Task AddAsync(TEntity entity);
         Task UpdateAsync(TEntity entity);
         Task DeleteAsync(Guid id); 
         #endregion
    }
}