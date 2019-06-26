using NextDoor.Core.MsSql;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<RefreshToken>> GetListForActiveTokenAsync(int uid)
            => await GetListAsync(predicate: r => r.Uid == uid && r.RevokedAt == null);

        public async Task AddAsync(RefreshToken token)
        {
            token.CreatedAt = DateTime.Now;
            Add(token);

            await CommitChangesAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAt = DateTime.Now;

            Update(token);

            await CommitChangesAsync();
        }
    }
}
