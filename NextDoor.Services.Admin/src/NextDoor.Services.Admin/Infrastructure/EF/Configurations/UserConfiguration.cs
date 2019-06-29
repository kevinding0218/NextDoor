using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextDoor.Services.Admin.Infrastructure.Domain;

namespace NextDoor.Services.Admin.Infrastructure.EF.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Mapping for table
            builder.ToTable("Users", "Identity");

            // Set identity for entity (auto increment)
            builder.Property(u => u.Id).HasColumnName("UID").UseSqlServerIdentityColumn();
            // Set key for entity
            builder.HasKey(u => u.Id);

            // Set mapping for columns
            builder.Property(u => u.Email).HasColumnType("nvarchar(100)").IsRequired();
            builder.Property(u => u.Role).HasColumnType("nvarchar(15)").IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(u => u.LastLogin).HasColumnType("datetime").IsRequired();

            builder.Property(u => u.CreatedBy).HasColumnType("int");
            builder.Property(u => u.CreatedOn).HasColumnType("datetime");
            builder.Property(u => u.LastUpdatedBy).HasColumnType("int");
            builder.Property(u => u.LastUpdatedOn).HasColumnType("datetime");

            builder.Property(u => u.Guid).HasColumnType("nvarchar(150)").IsRequired();
        }
    }
}
