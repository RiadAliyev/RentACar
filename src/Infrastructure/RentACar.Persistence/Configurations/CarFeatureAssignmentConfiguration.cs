using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class CarFeatureAssignmentConfiguration : IEntityTypeConfiguration<CarFeatureAssignment>
{
    public void Configure(EntityTypeBuilder<CarFeatureAssignment> b)
    {
        b.ToTable("CarFeatureAssignments");
        b.HasKey(x => x.Id);

        
        b.HasIndex(x => new { x.CarId, x.FeatureId }).IsUnique();

        b.HasOne(x => x.Car)
         .WithMany(c => c.Features)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Feature)
         .WithMany(f => f.Cars)
         .HasForeignKey(x => x.FeatureId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
