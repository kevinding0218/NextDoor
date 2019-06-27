using NextDoor.Core.Mongo;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.Mongo
{
    public class RefreshTokenMongoRepository : IRefreshTokenMongoRepository
    {
        private readonly IMongoRepository<RefreshToken> _repository;

        public RefreshTokenMongoRepository(IMongoRepository<RefreshToken> repository)
        {
            _repository = repository;
        }

        public async Task<RefreshToken> GetAsync(string token)
            => await _repository.GetSingleAsync(x => x.Token == token);

        public async Task AddAsync(RefreshToken token)
            => await _repository.AddAsync(token);

        public async Task RevokeOneAsync(RefreshToken token)
        {
            //token.RevokedAt = DateTime.Now;

            //await _repository.UpdateAsync(token);
            await _repository.DeleteAsync(token.Guid);
        }

        public async Task RevokeListAsync(IEnumerable<RefreshToken> tokens)
        {
            await _repository.DeleteRangeAsync(tokens.ToList().Select(t => t.Guid));
        }
    }
}
