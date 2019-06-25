using AutoMapper;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Services.Dto;

namespace NextDoor.Services.Identity.Services.AutoMapper
{
    public class IdentityMappings : Profile
    {
        public override string ProfileName => nameof(IdentityMappings);

        public IdentityMappings()
        {
            this.CreateMap<UserDto, User>()
                .ForMember(tar => tar.LastLogin, src => src.Ignore())
                .ForMember(tar => tar.CreatedBy, src => src.Ignore())
                .ForMember(tar => tar.CreatedOn, src => src.Ignore())
                .ForMember(tar => tar.LastUpdatedBy, src => src.Ignore())
                .ForMember(tar => tar.LastUpdatedOn, src => src.Ignore());

            this.CreateMap<User, UserDto>()
                .ForMember(tar => tar.PasswordTyped, src => src.Ignore());

            this.CreateMap<RefreshToken, RefreshTokenDto>()
                .ForMember(tar => tar.Revoked, src => src.Ignore())
                .ForMember(tar => tar.Role, src => src.Ignore());
            this.CreateMap<RefreshTokenDto, RefreshToken>();
        }
    }
}
