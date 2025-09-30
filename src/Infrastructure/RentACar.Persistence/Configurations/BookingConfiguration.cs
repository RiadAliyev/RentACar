using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> b)
    {
        b.ToTable("Bookings");
        b.HasKey(x => x.Id);

       
        b.Property(x => x.StartDate).HasColumnType("date");
        b.Property(x => x.EndDate).HasColumnType("date");

        b.Property(x => x.TotalPrice).HasPrecision(18, 2);
        b.Property(x => x.DepositAmount).HasPrecision(18, 2);

        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => x.Status).HasConversion<int>();

        b.HasIndex(x => new { x.CarId, x.StartDate, x.EndDate });
        b.HasIndex(x => new { x.CustomerId, x.StartDate });

        b.HasOne(x => x.Car)
         .WithMany(c => c.Bookings)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Customer)
         .WithMany(u => u.Bookings)
         .HasForeignKey(x => x.CustomerId)
         .OnDelete(DeleteBehavior.Cascade);

    }
}
