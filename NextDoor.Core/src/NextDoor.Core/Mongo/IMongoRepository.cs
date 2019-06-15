using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;
using NextDoor.Core.Types.Repository;

namespace NextDoor.Core.Mongo
{
    public interface IMongoRepository<TEntity> : INoSqlReadRepository<TEntity>, IAsyncCUDRepository<TEntity> where TEntity : class, IGuidIdentifiable
    {
    }
}