using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextDoor.Services.Identity.Infrastructure.Domain;

namespace NextDoor.Services.Identity.Infrastructure.EF.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Mapping for table
            builder.ToTable("RefreshToken", "Identity");

            // Set identity for entity (auto increment)
            builder.Property(r => r.Id).HasColumnName("RefreshTokenID").UseSqlServerIdentityColumn();
            // Set key for entity
            builder.HasKey(r => r.Id);

            // Set mapping for columns
            builder.Property(r => r.UID).HasColumnType("int").IsRequired();
            builder.Property(r => r.Token).HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(r => r.CreatedAt).HasColumnType("datetime");
            builder.Property(r => r.RevokedAt).HasColumnType("datetime");

            builder.Property(r => r.Guid).HasColumnType("nvarchar(150)").IsRequired();
        }
    }
}
