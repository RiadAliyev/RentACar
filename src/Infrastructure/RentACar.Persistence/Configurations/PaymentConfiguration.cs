using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.PaidAt).HasDefaultValueSql("GETUTCDATE()");
        b.Property(x => x.PaymentMethod).HasConversion<int>();

        b.HasIndex(x => new { x.BookingId, x.PaidAt });

        b.HasOne(x => x.Booking)
         .WithMany(bk => bk.Payments)
         .HasForeignKey(x => x.BookingId)
         .OnDelete(DeleteBehavior.Cascade); // booking silinərsə ödənişlər də silinsin

    }
}
