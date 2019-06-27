using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Core.Types.Repository
{
    public interface INonAsyncCUDRepository<TEntity> where TEntity : class
    {
        #region CREATE/UPDATE/DELETE
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(int id);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<int> CommitChangesAsync();
        #endregion
    }
}