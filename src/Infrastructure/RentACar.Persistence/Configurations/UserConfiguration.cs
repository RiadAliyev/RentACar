using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        b.Property(x => x.FullName).IsRequired().HasMaxLength(120);
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.Property(x => x.PhoneNumber).HasMaxLength(32);
        b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);


        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.PhoneNumber);

        // 1 - 1 : User <-> Company (optional)
        b.HasOne(x => x.Company)
         .WithOne(c => c.Owner)
         .HasForeignKey<Company>(c => c.OwnerId)
         .OnDelete(DeleteBehavior.Restrict); // user silinərsə company qalsın
    }
}
