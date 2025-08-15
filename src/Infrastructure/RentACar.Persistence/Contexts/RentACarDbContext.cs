using Microsoft.EntityFrameworkCore;
using RentACar.Domain.Entities;

namespace RentACar.Persistence.Contexts;

public class RentACarDbContext:DbContext
{
    public RentACarDbContext(DbContextOptions<RentACarDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentACarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarFeature> CarFeatures { get; set; }
    public DbSet<CarFeatureAssignment> CarFeatureAssignments { get; set; }
    public DbSet<CarImage> CarImages { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
}
