using NextDoor.Services.Identity.Infrastructure.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.Mongo
{
    public interface IRefreshTokenMongoRepository
    {
        Task<RefreshToken> GetAsync(string token);
        Task AddAsync(RefreshToken token);
        Task RevokeOneAsync(RefreshToken token);
        Task RevokeListAsync(IEnumerable<RefreshToken> tokens);
    }
}
