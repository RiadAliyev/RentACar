using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class CarsRepository:Repository<Car>,ICarsRepository
{
    public CarsRepository(RentACarDbContext context) : base(context)
    {

    }
}
