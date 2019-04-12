using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.SqlSever
{
    public class SqlServerRepository<TEntity> : ISqlServerRepository<TEntity> where TEntity : class, IIdentifiable
    {
        protected DbContext _dbContext;
        private readonly IQueryable<TEntity> _collection;

        public SqlServerRepository(DbContext dbContext, string collectionName)
        {
            this._dbContext = dbContext;
            this._collection = dbContext.Set<TEntity>() as IQueryable<TEntity>;
        }
        public async Task<TEntity> GetSingleAsync(Guid id)
            => await GetSingleAsync(e => e.Id == id);

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Where(predicate).AsNoTracking().SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Where(predicate).AsNoTracking().ToListAsync();

        public Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistedAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase
        {
            throw new NotImplementedException();
        }
    }
}