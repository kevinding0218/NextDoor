using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Messages.Queries;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Infrastructure.EF.Repositories
{
    public interface IUserEFRepository
    {
        Task<PagedResult<User>> BrowseAsync(BrowseUserQuery query);
    }
}
