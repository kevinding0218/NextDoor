using NextDoor.Core.Types;
using NextDoor.Core.Types.Repository;
using MongoDB.Driver;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using NextDoor.Core.Types.Pagination;
using MongoDB.Driver.Linq;

namespace NextDoor.Core.Mongo
{
    public class MongoRepository<TEntity> : IAsyncCUDRepository<TEntity> where TEntity : class, IIdentifiable
    {
        protected IMongoCollection<TEntity> _collection {get;}

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            this._collection = database.GetCollection<TEntity>(collectionName);
        }

        #region CRUD
        public async Task<TEntity> GetSingleAsync(Guid id)
            => await GetSingleAsync(e => e.Id == id);

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).SingleOrDefaultAsync();
        
        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
            => await this._collection.Find(predicate).ToListAsync();

        public async Task AddAsync(TEntity entity) => await this._collection.InsertOneAsync(entity);
        public async Task UpdateAsync(TEntity entity) => await this._collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
        public async Task DeleteAsync(Guid id) => await this._collection.DeleteOneAsync(e => e.Id == id);    
        #endregion

        #region HELPER
        public async Task<bool> IsExistedAsync(Expression<Func<TEntity, bool>> predicate) => await this._collection.Find(predicate).AnyAsync();

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate, TQuery query) where TQuery : PagedQueryBase
            => await this._collection.AsQueryable().Where(predicate).PaginateAsync(query);
        #endregion
    }
}