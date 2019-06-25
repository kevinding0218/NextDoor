using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Types;
using System;

namespace NextDoor.Services.Identity.Services.Dto
{
    public class RefreshTokenDto
    {
        public int Id { get; private set; }
        public int Uid { get; private set; }
        public string Token { get; private set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string Guid { get; set; }
        public bool Revoked => RevokedAt.HasValue;

        protected RefreshTokenDto()
        {
        }

        public RefreshTokenDto(UserDto userDto, IPasswordHasher<UserDto> passwordHasher)
        {
            Uid = userDto.Id;
            CreatedAt = DateTime.UtcNow;
            Token = CreateToken(userDto, passwordHasher);
        }

        private static string CreateToken(UserDto userDto, IPasswordHasher<UserDto> passwordHasher)
            => passwordHasher.HashPassword(userDto, System.Guid.NewGuid().ToString("N"))
                .Replace("=", string.Empty)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty);

        public void Revoke()
        {
            if (Revoked)
            {
                throw new NextDoorException(IdentityExceptionCode.RefreshTokenAlreadyRevoked,
                    $"Refresh token: '{Token}' was already revoked at '{RevokedAt}'.");
            }
        }
    }
}
