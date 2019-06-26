using NextDoor.Core.MsSql;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public class UserRepository : MsSqlRepository<User>, IUserRepository
    {
        public UserRepository(NextDoorDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetAsync(int id)
            => await GetSingleAsync(id);

        public async Task<User> GetAsync(string email)
            => await GetSingleAsync(predicate: u => u.Email.ToLowerInvariant() == email.ToLowerInvariant());

        public async Task AddAsync(User user)
        {
            user.LastLogin = DateTime.Now;
            Add(user);

            await CommitChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            Update(user);

            await CommitChangesAsync();
        }
    }
}
