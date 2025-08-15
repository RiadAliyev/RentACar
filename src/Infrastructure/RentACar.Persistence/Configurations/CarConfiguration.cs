using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> b)
    {
        b.ToTable("Cars");
        b.HasKey(x => x.Id);

        b.Property(x => x.Brand).IsRequired().HasMaxLength(80);
        b.Property(x => x.Model).IsRequired().HasMaxLength(80);
        b.Property(x => x.Year).IsRequired();

        b.Property(x => x.DailyPrice).HasPrecision(18, 2);
        b.Property(x => x.Location).IsRequired().HasMaxLength(160);

        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Enum-lar default int kimi saxlanır; aydınlıq üçün qeyd:
        b.Property(x => x.TransmissionType).HasConversion<int>();
        b.Property(x => x.FuelType).HasConversion<int>();

        b.HasIndex(x => new { x.Brand, x.Model, x.Year });
        b.HasIndex(x => x.Location);

        // Car -> Owner (User)  many-to-1
        b.HasOne(x => x.Owner)
         .WithMany(u => u.Cars)
         .HasForeignKey(x => x.OwnerId)
         .OnDelete(DeleteBehavior.Restrict); // user silinməsi maşınları silməsin

        // Car -> Company (optional) many-to-1 yuxarıda Company-də də bağlıdır
        b.HasOne(x => x.Company)
         .WithMany(cmp => cmp.Cars)
         .HasForeignKey(x => x.CompanyId)
         .OnDelete(DeleteBehavior.SetNull);

        // Car -> Images
        b.HasMany(x => x.Images)
         .WithOne(i => i.Car)
         .HasForeignKey(i => i.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        // Car -> Features (via Assignment)
        b.HasMany(x => x.Features)
         .WithOne(a => a.Car)
         .HasForeignKey(a => a.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        // Car -> Bookings
        b.HasMany(x => x.Bookings)
         .WithOne(bk => bk.Car)
         .HasForeignKey(bk => bk.CarId)
         .OnDelete(DeleteBehavior.Cascade);

        // Car -> Reviews
        b.HasMany(x => x.Reviews)
         .WithOne(r => r.Car)
         .HasForeignKey(r => r.CarId)
         .OnDelete(DeleteBehavior.Cascade);

    }
}

