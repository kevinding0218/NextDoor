using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Services.Dto;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public interface ITokenService
    {
        Task<RefreshTokenDto> CreateNewRefreshTokenAsync(int userId);
        Task<JsonWebToken> CreateNewJwtAccessTokenAsync(int Uid, string Role, string refreshToken);
        Task<JsonWebToken> RefreshExistedJwtAccessTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken, int userId);
    }
}
