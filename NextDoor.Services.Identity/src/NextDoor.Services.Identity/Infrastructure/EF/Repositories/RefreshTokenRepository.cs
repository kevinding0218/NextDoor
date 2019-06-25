using NextDoor.Core.MsSql;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Repositories
{
    public class RefreshTokenRepository : MsSqlRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(NextDoorDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<RefreshToken> GetAsync(string token)
            => await GetSingleAsync(predicate: r => r.Token == token);

        public async Task AddAsync(RefreshToken token)
        {
            token.CreatedAt = DateTime.UtcNow;
            Add(token);

            await CommitChangesAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAt = DateTime.UtcNow;

            Update(token);

            await CommitChangesAsync();
        }
    }
}
