using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextDoor.Core.MSSQL;
using NextDoor.Services.Identity.Core.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF
{
    public class NextDoorDbContext : DbContext
    {
        private readonly IOptions<MsSqlDbOptions> _sqlOptions;

        public NextDoorDbContext(IOptions<MsSqlDbOptions> sqlOptions)
        {
            _sqlOptions = sqlOptions;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            if (_sqlOptions.Value.InMemory)
            {
                optionsBuilder.UseInMemoryDatabase(_sqlOptions.Value.Database);

                return;
            }

            optionsBuilder.UseSqlServer(_sqlOptions.Value.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
