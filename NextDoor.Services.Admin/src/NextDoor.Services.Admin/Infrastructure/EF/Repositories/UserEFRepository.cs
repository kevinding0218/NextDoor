using NextDoor.Core.MsSql;
using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Messages.Queries;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Infrastructure.EF.Repositories
{
    public class UserEFRepository : MsSqlRepository<User>, IUserEFRepository
    {
        public UserEFRepository(NextDoorDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PagedResult<User>> BrowseAsync(BrowseUserQuery query)
        {
            if (string.IsNullOrEmpty(query.EmailDomain))
            {
                return await BrowseAsync(u => 1 == 1, query);
            }
            else
            {
                return await BrowseAsync(u => u.Email.Contains(query.EmailDomain), query);
            }
        }
    }
}
