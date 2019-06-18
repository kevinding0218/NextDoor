using NextDoor.Core.Types.Domain;
using NextDoor.Core.Types.Repository;

namespace NextDoor.Core.Mongo
{
    public interface IMongoRepository<TEntity> : INoSqlReadRepository<TEntity>, IAsyncCUDRepository<TEntity> where TEntity : class, IGuidIdentifiable
    {
    }
}