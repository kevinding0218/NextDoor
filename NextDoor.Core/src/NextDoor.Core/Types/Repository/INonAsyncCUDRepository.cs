using System.Threading.Tasks;

namespace NextDoor.Core.Types.Repository
{
    public interface INonAsyncCUDRepository<TEntity> where TEntity : class
    {
        #region CREATE/UPDATE/DELETE
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        Task<int> CommitChangesAsync();
        #endregion
    }
}