using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.Mongo
{
    public interface IUserMongoRepository
    {
        Task<User> GetAsync(Guid id);
        Task<User> GetAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
