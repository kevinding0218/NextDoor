using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Repository;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Core.MsSql
{
    public class MsSqlRepository<TEntity> : INonAsyncCUDRepository<TEntity> where TEntity : class, IIdentifiable
    {
        protected DbContext _dbContext;
        private readonly DbSet<TEntity> _collection;

        public MsSqlRepository(DbContext dbContext, string collectionName)
        {
            this._dbContext = dbContext;
            this._collection = dbContext.Set<TEntity>();
        }
        #region CRUD
        public async Task<TEntity> GetSingleAsync(Guid id)
            => await GetSingleAsync(e => e.Id == id);

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await (this._collection as IQueryable<TEntity>).Where(predicate).AsNoTracking().SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
            => await (this._collection as IQueryable<TEntity>).Where(predicate).AsNoTracking().ToListAsync();

        /// The method AddAsync() is async only to allow special value generators, such as the one used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo', 
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// Update and Remove are the same as Add in as much as they only affect the internal tracking until you save the changes you've made.
        public void Add(TEntity entity)
            => this._collection.Add(entity);

        public void Update(TEntity entity)
            => this._collection.Update(entity);

        ///If you dont want to query for it just create an entity through constructor, and then delete it.
        public void Delete(Guid id) {
            // => this._collection.Remove(this._collection.SingleOrDefault(e => e.Id == id));
            var instance = Activator.CreateInstance(typeof(TEntity), new object[] { id }) as TEntity;
            this._collection.Attach(instance);
            this._collection.Remove(instance);
        }
            
        public void Delete(TEntity entity)
            => this._collection.Remove(entity);
        #endregion

        #region Unit Of Work
        public async Task<int> CommitChangesAsync()
        {
            return await this._dbContext.SaveChangesAsync();
        }
        #endregion

        #region HELPER
        public async Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Where(predicate).AnyAsync();
        
        public Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase
        {
            // => this._collection.AsQueryable().Where(predicate).PaginateAsync(query);
            throw new NotImplementedException();
        }
        #endregion

        #region Dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}