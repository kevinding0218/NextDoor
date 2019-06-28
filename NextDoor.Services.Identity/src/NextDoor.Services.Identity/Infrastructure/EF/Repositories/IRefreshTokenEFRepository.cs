using NextDoor.Services.Identity.Infrastructure.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public interface IRefreshTokenEFRepository
    {
        Task<RefreshToken> GetAsync(string token);
        Task<IEnumerable<RefreshToken>> GetListForActiveTokenAsync(int uid);
        Task AddAsync(RefreshToken token);
        Task RevokeOneAsync(RefreshToken token);
        Task RevokeListAsync(IEnumerable<RefreshToken> tokens);
    }
}
