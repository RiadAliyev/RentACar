using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;
namespace RentACar.Persistence.Repositories;

public  class CarImageRepository:Repository<CarImage>,ICarImageRepository
{
    public CarImageRepository(RentACarDbContext context) : base(context)
    {

    }
}
