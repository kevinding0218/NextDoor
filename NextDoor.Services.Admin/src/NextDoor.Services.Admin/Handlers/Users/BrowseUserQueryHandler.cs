using NextDoor.Core.Handlers;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Dto;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Infrastructure.EF.Repositories;
using NextDoor.Services.Admin.Infrastructure.Mongo;
using NextDoor.Services.Admin.Messages.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Handlers.Users
{
    public class BrowseUserQueryHandler : IQueryHandler<BrowseUserQuery, PagedResult<UserDto>>
    {
        private readonly IUserEFRepository _userEFRepository;
        private readonly IUserMongoRepository _userMongoRepository;

        public BrowseUserQueryHandler(IUserEFRepository userEFRepository, IUserMongoRepository userMongoRepository)
        {
            _userEFRepository = userEFRepository;
            _userMongoRepository = userMongoRepository;
        }

        public async Task<PagedResult<UserDto>> HandleAsync(BrowseUserQuery query)
        {
            var pagedResult = (PagedResult<User>)null;
            if (Shared.UseSql)
            {
                pagedResult = await _userEFRepository.BrowseAsync(query);
            }
            else
            {
                pagedResult = await _userMongoRepository.BrowseAsync(query);
            }

            var users = pagedResult.Items.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                LastLogin = u.LastLogin,
                Guid = u.Guid
            });

            return PagedResult<UserDto>.From(pagedResult, users);
        }
    }
}
