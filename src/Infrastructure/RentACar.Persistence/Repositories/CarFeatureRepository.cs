using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class CarFeatureRepository:Repository<CarFeature>,ICarFeatureRepository
{
    public CarFeatureRepository(RentACarDbContext context) : base(context)
    {

    }
}
