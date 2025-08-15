using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class UserRepository:Repository<User>,IUserRepository
{
    public UserRepository(RentACarDbContext context) : base(context)
    {

    }
}
