using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class BioConfiguration : IEntityTypeConfiguration<Bio>
{
    public void Configure(EntityTypeBuilder<Bio> b)
    {
        b.ToTable("Bios");
        b.HasKey(x => x.Id);

        b.Property(x => x.Key)
         .IsRequired()
         .HasMaxLength(100);

        b.Property(x => x.Value)
         .IsRequired()
         .HasMaxLength(2000);

        b.Property(x => x.ValueType)
         .HasMaxLength(50);

        b.Property(x => x.CreatedAt)
         .HasDefaultValueSql("GETUTCDATE()");

        b.HasIndex(x => x.Key).IsUnique();
    }
}
