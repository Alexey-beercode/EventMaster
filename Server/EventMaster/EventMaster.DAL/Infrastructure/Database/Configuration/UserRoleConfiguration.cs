using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMaster.DAL.Infrastructure.Database.Configuration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.UserId)
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .IsRequired();

            builder.Property(ur => ur.IsDeleted)
                .IsRequired();

            builder.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();
        }
    }
}