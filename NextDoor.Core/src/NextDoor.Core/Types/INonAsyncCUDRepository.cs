using System;

namespace NextDoor.Core.Types
{
    public interface INonAsyncCUDRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IIdentifiable
    {
         #region CREATE/UPDATE/DELETE
         void Add(TEntity entity);
         void Update(TEntity entity);
         void Delete(Guid id);
         #endregion
    }
}