using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMaster.DAL.Infrastructure.Database.Configuration
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable("Participants");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.BirthDate)
                .IsRequired();

            builder.Property(p => p.RegistrationDate)
                .IsRequired();

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.EventId)
                .IsRequired();

            builder.Property(p => p.IsDeleted)
                .IsRequired();

            builder.HasIndex(p => p.Email)
                .IsUnique();

            builder.HasOne<User>() 
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Event>()
                .WithMany()
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}