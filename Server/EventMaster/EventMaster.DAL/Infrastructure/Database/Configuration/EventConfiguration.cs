using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMaster.DAL.Infrastructure.Database.Configuration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");
            
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);  

            builder.Property(e => e.Description)
                .HasMaxLength(500);  

            builder.Property(e => e.Date)
                .IsRequired();

            builder.Property(e => e.MaxParticipants)
                .IsRequired();

            builder.Property(e => e.Image)
                .HasColumnType("bytea"); 

            builder.Property(e => e.CategoryId)
                .IsRequired();

            builder.Property(e => e.IsDeleted)
                .IsRequired();
            
            builder.OwnsOne(e => e.Location, location =>
            {
                location.Property(l => l.City).IsRequired().HasMaxLength(100);
                location.Property(l => l.Street).IsRequired().HasMaxLength(100);
                location.Property(l => l.Building).IsRequired().HasMaxLength(50);
            });

            // Настройка индексов
            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.Name);
        }
    }
}