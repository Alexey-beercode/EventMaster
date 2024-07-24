using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMaster.DAL.Infrastructure.Database.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256); 

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(256); 

            builder.Property(u => u.IsDeleted)
                .IsRequired();

            builder.HasIndex(u => u.Login)
                .IsUnique();
        }
    }
}