using Microsoft.AspNetCore.Identity;

namespace RentACar.Domain.Entities;
public class AppUser: IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime ExpireDate { get; set; }
    public Company? Company { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

