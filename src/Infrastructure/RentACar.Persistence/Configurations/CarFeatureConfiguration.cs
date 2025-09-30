using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class CarFeatureConfiguration : IEntityTypeConfiguration<CarFeature>
{
    public void Configure(EntityTypeBuilder<CarFeature> b)
    {
        b.ToTable("CarFeatures");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
         .IsRequired()
         .HasMaxLength(120);

        b.HasIndex(x => x.Name).IsUnique();

        b.HasMany(x => x.Cars)
         .WithOne(a => a.Feature)
         .HasForeignKey(a => a.FeatureId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
