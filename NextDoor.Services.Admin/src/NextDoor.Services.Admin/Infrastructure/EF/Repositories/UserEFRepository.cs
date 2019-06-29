using NextDoor.Core.MsSql;
using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Messages.Queries;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Infrastructure.EF.Repositories
{
    public class UserEFRepository : MsSqlRepository<User>, IUserEFRepository
    {
        public UserEFRepository(NextDoorDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PagedResult<User>> BrowseAsync(BrowseUserQuery query)
            => await BrowseAsync(u => u.CreatedOn < DateTime.Now.AddDays(1), query);
    }
}
