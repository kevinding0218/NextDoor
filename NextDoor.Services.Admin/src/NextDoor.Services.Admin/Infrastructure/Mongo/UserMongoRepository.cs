using NextDoor.Core.Mongo;
using NextDoor.Core.Types.Pagination;
using NextDoor.Services.Admin.Infrastructure.Domain;
using NextDoor.Services.Admin.Messages.Queries;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Infrastructure.Mongo
{
    public class UserMongoRepository : IUserMongoRepository
    {
        private readonly IMongoRepository<User> _repository;

        public UserMongoRepository(IMongoRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<User>> BrowseAsync(BrowseUserQuery query)
        {
            if (string.IsNullOrEmpty(query.EmailDomain))
            {
                return await _repository.BrowseAsync(u => 1 == 1, query);
            }
            else
            {
                return await _repository.BrowseAsync(u => u.Email.Contains(query.EmailDomain), query);
            }
        }
    }
}
