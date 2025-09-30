using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.ToTable("Reviews");
        b.HasKey(x => x.Id);

        b.Property(x => x.Rating).IsRequired();             
        b.Property(x => x.Comment).HasMaxLength(1000);
        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Hər user bir maşını 1 dəfə qiymətləndirə bilsin
        b.HasIndex(x => new { x.CarId, x.CustomerId }).IsUnique();

        b.HasOne(x => x.Car)
         .WithMany(c => c.Reviews)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Customer)
         .WithMany(u => u.Reviews)
         .HasForeignKey(x => x.CustomerId)
         .OnDelete(DeleteBehavior.Cascade);

        
    }
}
