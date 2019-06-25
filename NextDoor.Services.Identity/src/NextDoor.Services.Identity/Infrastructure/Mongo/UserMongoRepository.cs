using NextDoor.Core.Mongo;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.Mongo
{
    public class UserMongoRepository : IUserMongoRepository
    {
        private readonly IMongoRepository<User> _repository;

        public UserMongoRepository(IMongoRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<User> GetAsync(Guid id)
            => await _repository.GetSingleAsync(id);

        public async Task<User> GetAsync(string email)
            => await _repository.GetSingleAsync(x => x.Email == email.ToLowerInvariant());

        public async Task AddAsync(User user)
            => await _repository.AddAsync(user);

        public async Task UpdateAsync(User user)
            => await _repository.UpdateAsync(user);
    }
}
