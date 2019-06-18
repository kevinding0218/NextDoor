using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextDoor.Services.Identity.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.EF.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Mapping for table
            builder.ToTable("Users", "Indentity");

            // Set identity for entity (auto increment)
            builder.Property(u => u.Id).HasColumnName("UID").UseSqlServerIdentityColumn();
            // Set key for entity
            builder.HasKey(u => u.Id);

            // Set mapping for columns
            builder.Property(p => p.Email).HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(p => p.Role).HasColumnType("nvarchar(15)").IsRequired();
            builder.Property(p => p.PasswordHash).HasColumnType("nvarchar(200)").IsRequired();
            //builder.Property(p => p.FirstName).HasColumnType("nvarchar(30)").IsRequired();
            //builder.Property(p => p.MiddleInitial).HasColumnType("nvarchar(10)");
            //builder.Property(p => p.LastName).HasColumnType("nvarchar(30)").IsRequired();
            //builder.Property(p => p.Active).HasColumnType("bit").IsRequired().HasDefaultValue(1);
            builder.Property(p => p.LastLogin).HasColumnType("datetime").IsRequired();

            builder.Property(p => p.CreatedBy).HasColumnType("int");
            builder.Property(p => p.CreatedOn).HasColumnType("datetime");
            builder.Property(p => p.LastUpdatedBy).HasColumnType("int");
            builder.Property(p => p.LastUpdatedOn).HasColumnType("datetime");
        }
    }
}
