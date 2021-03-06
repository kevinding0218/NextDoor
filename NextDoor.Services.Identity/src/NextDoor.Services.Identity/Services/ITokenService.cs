﻿using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Services.Dto;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public interface ITokenService
    {
        Task<RefreshTokenDto> CreateNewRefreshTokenAsync(int Uid);
        Task<JsonWebToken> CreateNewJwtAccessTokenAsync(int Uid, string Role, string refreshToken);
        Task<JsonWebToken> RenewExistedJwtAccessTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken, int userId);
        Task RevokeAllExistedRefreshTokenAsync(int userId);
    }
}
