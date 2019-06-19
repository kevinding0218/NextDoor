using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Domain;
using NextDoor.Core.Types.Repository;

namespace NextDoor.Core.MsSql
{
    public interface IMsSqlRepository<TEntity> : ISqlReadRepository<TEntity>, INonAsyncCUDRepository<TEntity> where TEntity : class, IIdIdentifiable
    {
    }
}