using NextDoor.Services.Identity.Infrastructure.Domain;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetAsync(string token);
        Task AddAsync(RefreshToken token);
        Task RevokeAsync(RefreshToken token);
    }
}
