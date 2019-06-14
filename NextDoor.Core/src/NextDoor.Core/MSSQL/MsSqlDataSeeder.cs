using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextDoor.Core.MSSQL
{
    public class MsSqlDataSeeder<dbContext> : IDataSeeder where dbContext : DbContext
    {
        private readonly dbContext _dbContext;
        private readonly IInitializer _initializer;
        private readonly IOptions<MsSqlDbOptions> _msSqlOptions;

        public MsSqlDataSeeder(dbContext dbContext, IInitializer initializer, IOptions<MsSqlDbOptions> msSqlOptions)
        {
            _dbContext = dbContext;
            _initializer = initializer;
            _msSqlOptions = msSqlOptions;
        }

        public async Task SeedAsync()
        {
            if (_msSqlOptions.Value.InMemory)
            {
                await _initializer.InitializeAsync();

                return;
            }

            await _dbContext.Database.EnsureCreatedAsync();
            await _dbContext.Database.MigrateAsync();

            await ExecuteAsync(() => _initializer.InitializeAsync());
        }

        protected async Task ExecuteAsync(Func<Task> query)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await query();
                transaction.Commit();
            }
        }
    }
}
