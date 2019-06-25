using NextDoor.Core.Mongo;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
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

        public async Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(token);
        }
    }
}
