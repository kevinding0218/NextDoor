using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NextDoor.Core.Types.Domain;
using NextDoor.Core.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NextDoor.Core.MsSql
{
    public class MsSqlRepository<TEntity> : IMsSqlRepository<TEntity> where TEntity : class, IIdIdentifiable, IGuidIdentifiable
    {
        protected IUserInfo _userInfo;
        protected DbContext _dbContext;
        private readonly DbSet<TEntity> _collection;

        public MsSqlRepository(DbContext dbContext)
        {
            this._dbContext = dbContext;
            this._collection = dbContext.Set<TEntity>();
        }

        public MsSqlRepository(DbContext dbContext, IUserInfo userInfo)
        {
            _dbContext = dbContext;
            this._collection = dbContext.Set<TEntity>();
            _userInfo = userInfo;
        }
        #region CRUD
        public async Task<TEntity> GetSingleAsync(int id) => await GetSingleAsync(selector: (TEntity) => TEntity, predicate: e => e.Id == id);

        public async Task<TEntity> GetSingleAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
            => await GetSingleAsync(selector: (TEntity) => TEntity, predicate: predicate, orderBy: orderBy, include: include, disableTracking: disableTracking);

        public async Task<TResult> GetSingleAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = this._collection;

            if (selector == null) selector = entity => entity is TResult ? (TResult)(object)entity : default(TResult);

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).Select(selector).SingleOrDefaultAsync();
            return await query.Select(selector).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
            => await GetListAsync(selector: (TEntity) => TEntity, predicate: predicate, orderBy: orderBy, include: include, disableTracking: disableTracking);

        public async Task<IEnumerable<TResult>> GetListAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector = null,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<TEntity> query = this._collection;

            if (selector == null) selector = entity => entity is TResult ? (TResult)(object)entity : default(TResult);

            if (disableTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).Select(selector).ToListAsync();
            return await query.Select(selector).ToListAsync();
        }

        /// The method AddAsync() is async only to allow special value generators, such as the one used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo', 
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// Update and Remove are the same as Add in as much as they only affect the internal tracking until you save the changes you've made.
        public void Add(TEntity entity)
        {
            AttachAddProperty(entity);

            this._collection.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(e =>
            {
                AttachAddProperty(e);
            });

            this._collection.AddRange(entities);
        }

        private void AttachAddProperty(TEntity entity)
        {
            var guidEntity = entity as IGuidIdentifiable;
            if (guidEntity.Guid == Guid.Empty)
                guidEntity.Guid = Guid.NewGuid();


            if (entity is IAuditableEntity auditEntity)
            {
                auditEntity.CreatedBy = auditEntity.LastUpdatedBy = _userInfo == null ? auditEntity.CreatedBy : _userInfo.UID;
                if (auditEntity.CreatedOn == null) auditEntity.CreatedOn = DateTime.Now;
                if (auditEntity.LastUpdatedOn == null) auditEntity.LastUpdatedOn = DateTime.Now;
            }
        }

        public void Update(TEntity entity)
        {
            AttachUpdateProperty(entity);
            this._collection.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(e =>
            {
                AttachUpdateProperty(e);
            });

            this._collection.UpdateRange(entities);
        }

        private void AttachUpdateProperty(TEntity entity)
        {
            if (entity is IAuditableEntity auditEntity)
            {
                auditEntity.LastUpdatedBy = _userInfo == null ? auditEntity.LastUpdatedBy : _userInfo.UID;
                if (auditEntity.LastUpdatedOn == null) auditEntity.LastUpdatedOn = DateTime.Now;
            }
        }

        ///If you dont want to query for it just create an entity through constructor, and then delete it.
        public void Delete(int id)
        {
            // => this._collection.Remove(this._collection.SingleOrDefault(e => e.Id == id));
            var instance = Activator.CreateInstance(typeof(TEntity), new object[] { id }) as TEntity;
            this._collection.Attach(instance);
            this._collection.Remove(instance);
        }

        public void Delete(TEntity entity) => this._collection.Remove(entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void DeleteRange(IEnumerable<TEntity> entities) => this._collection.RemoveRange(entities);
        #endregion

        #region Unit Of Work
        public async Task<int> CommitChangesAsync()
        {
            return await this._dbContext.SaveChangesAsync();
        }
        #endregion

        #region HELPER
        public async Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate) => await this._collection.Where(predicate).AnyAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase => await this._collection.AsQueryable().Where(predicate).PaginateAsync(query);
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