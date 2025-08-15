using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> b)
    {
        b.ToTable("CarImages");
        b.HasKey(x => x.Id);

        b.Property(x => x.ImageUrl)
         .IsRequired()
         .HasMaxLength(500);

        b.HasIndex(x => x.CarId);
    }
}
