using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NextDoor.Core.Types.Domain;
using NextDoor.Core.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NextDoor.Core.Mongo
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class, IGuidIdentifiable
    {
        protected IUserInfo _userInfo;
        protected IMongoCollection<TEntity> _collection { get; }

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            this._collection = database.GetCollection<TEntity>(collectionName);
        }

        public MongoRepository(IMongoDatabase database, string collectionName, IUserInfo userInfo)
        {
            this._collection = database.GetCollection<TEntity>(collectionName);
            _userInfo = userInfo;
        }

        #region CRUD
        public async Task<TEntity> GetSingleAsync(Guid guid)
            => await GetSingleAsync(e => e.Guid == guid);

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).ToListAsync();

        public async Task AddAsync(TEntity entity)
        {
            AttachAddProperty(entity);

            await this._collection.InsertOneAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(e =>
            {
                AttachAddProperty(e);
            });

            await this._collection.InsertManyAsync(entities);
        }

        private void AttachAddProperty(TEntity entity)
        {
            var guidEntity = entity as IGuidIdentifiable;
            if (guidEntity.Guid == null)
                guidEntity.Guid = Guid.NewGuid();

            var auditEntity = entity as IAuditableEntity;

            if (auditEntity != null)
            {
                auditEntity.CreatedBy = auditEntity.LastUpdatedBy = _userInfo == null ? auditEntity.CreatedBy : _userInfo.UID;
                if (auditEntity.CreatedOn == null) auditEntity.CreatedOn = DateTime.Now;
                if (auditEntity.LastUpdatedOn == null) auditEntity.LastUpdatedOn = DateTime.Now;
            }
        }

        public async Task UpdateAsync(TEntity entity) => await this._collection.ReplaceOneAsync(e => e.Guid == entity.Guid, entity);

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AttachUpdateProperty(entity);
                await this._collection.ReplaceOneAsync(e => e.Guid == entity.Guid, entity);
            }
        }

        private void AttachUpdateProperty(TEntity entity)
        {
            var auditEntity = entity as IAuditableEntity;
            if (auditEntity != null)
            {
                auditEntity.LastUpdatedBy = _userInfo == null ? auditEntity.LastUpdatedBy : _userInfo.UID;
                if (auditEntity.LastUpdatedOn == null) auditEntity.LastUpdatedOn = DateTime.Now;
            }
        }
        public async Task DeleteAsync(Guid guid) => await this._collection.DeleteOneAsync(e => e.Guid == guid);
        public async Task DeleteRangeAsync(IEnumerable<Guid> guids)
        {
            foreach (var g in guids)
            {
                await this._collection.DeleteOneAsync(e => e.Guid == g);
            }
        }
        #endregion

        #region HELPER
        public async Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate) => await this._collection.Find(predicate).AnyAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase
            => await this._collection.AsQueryable().Where(predicate).PaginateAsync(query);
        #endregion
    }
}