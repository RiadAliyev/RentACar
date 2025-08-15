using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class ReviewRepository:Repository<Review>,IReviewRepository
{
    public ReviewRepository(RentACarDbContext context) : base(context)
    {

    }
}
