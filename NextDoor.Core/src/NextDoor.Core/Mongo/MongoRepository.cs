using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NextDoor.Core.Types.Domain;
using NextDoor.Core.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NextDoor.Core.Mongo
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class, IGuidIdentifiable
    {
        protected IMongoCollection<TEntity> _collection { get; }

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            this._collection = database.GetCollection<TEntity>(collectionName);
        }

        #region CRUD
        public async Task<TEntity> GetSingleAsync(Guid guid)
            => await GetSingleAsync(e => e.Guid == guid);

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).ToListAsync();

        public async Task AddAsync(TEntity entity) => await this._collection.InsertOneAsync(entity);
        public async Task UpdateAsync(TEntity entity) => await this._collection.ReplaceOneAsync(e => e.Guid == entity.Guid, entity);
        public async Task DeleteAsync(Guid guid) => await this._collection.DeleteOneAsync(e => e.Guid == guid);
        #endregion

        #region HELPER
        public async Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate) => await this._collection.Find(predicate).AnyAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase
            => await this._collection.AsQueryable().Where(predicate).PaginateAsync(query);
        #endregion
    }
}