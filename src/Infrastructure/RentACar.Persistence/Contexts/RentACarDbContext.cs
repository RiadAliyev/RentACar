using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Contexts;

public class RentACarDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public RentACarDbContext(DbContextOptions<RentACarDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1) Identity mapping-ləri
        base.OnModelCreating(modelBuilder);

        // 2) Sənin ayrıca IEntityTypeConfiguration faylların varsa yüklə
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentACarDbContext).Assembly);

        // 3) Car əlaqələri
        modelBuilder.Entity<Car>(b =>
        {
            // Car -> Owner (mütləq)
            b.HasOne(c => c.Owner)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Car -> Company (opsional)
            b.HasOne(c => c.Company)
                .WithMany(co => co.Cars)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Yalnız biri dolu olsun: (CompanyId != NULL XOR OwnerId != NULL)
            b.HasCheckConstraint(
                "CK_Car_OwnerOrCompany",
                "((CompanyId IS NOT NULL AND OwnerId IS NULL) OR (CompanyId IS NULL AND OwnerId IS NOT NULL))"
                );
        });

        // 4) Company ↔ AppUser (1:1) – bir userin yalnız bir şirkəti olsun
        modelBuilder.Entity<Company>(b =>
        {
            b.HasOne(c => c.Owner)
                .WithOne(u => u.Company)
                .HasForeignKey<Company>(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // (istəsən) OwnerId üzərindən unikallıq
            b.HasIndex(c => c.OwnerId).IsUnique();
        });
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarFeature> CarFeatures { get; set; }
    public DbSet<CarFeatureAssignment> CarFeatureAssignments { get; set; }
    public DbSet<CarImage> CarImages { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
}
