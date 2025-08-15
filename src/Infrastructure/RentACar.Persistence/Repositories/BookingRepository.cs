using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class BookingRepository:Repository<Booking>,IBookingRepository
{
    public BookingRepository(RentACarDbContext context) : base(context)
    {

    }
}
