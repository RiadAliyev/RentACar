using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> b)
    {
        b.ToTable("Companies");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(160);
        b.Property(x => x.RegistrationNumber).IsRequired().HasMaxLength(60);
        b.Property(x => x.Address).IsRequired().HasMaxLength(300);

        b.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        b.HasIndex(x => x.RegistrationNumber).IsUnique();
        b.HasIndex(x => x.Name);

        // 1 - many : Company -> Cars
        b.HasMany(x => x.Cars)
         .WithOne(c => c.Company)
         .HasForeignKey(c => c.CompanyId)
         .OnDelete(DeleteBehavior.SetNull); // şirkət silinərsə maşınlar qalır, CompanyId = NULL
    }
}
