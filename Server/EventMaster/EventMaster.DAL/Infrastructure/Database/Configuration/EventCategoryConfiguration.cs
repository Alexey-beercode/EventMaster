using EventMaster.Domain.Entities.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventMaster.DAL.Infrastructure.Database.Configuration;

public class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
{
    public void Configure(EntityTypeBuilder<EventCategory> builder)
    {
        builder.ToTable("EventCategories");
        
        builder.HasKey(ec => ec.Id);
        
        builder.Property(ec => ec.Name)
            .IsRequired()
            .HasMaxLength(100); 

        builder.Property(ec => ec.IsDeleted)
            .IsRequired();
        
    }
}