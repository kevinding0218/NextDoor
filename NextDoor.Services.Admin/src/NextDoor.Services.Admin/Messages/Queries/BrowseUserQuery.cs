using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Dto;

namespace NextDoor.Services.Admin.Messages.Queries
{
    public class BrowseUserQuery : PagedQueryBase, IQuery<PagedResult<UserDto>>
    {
        public string EmailDomain { get; set; }
    }
}
