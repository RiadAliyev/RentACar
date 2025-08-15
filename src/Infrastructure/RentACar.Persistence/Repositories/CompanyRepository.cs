using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class CompanyRepository:Repository<Company>,ICompanyRepository
{
    public CompanyRepository(RentACarDbContext context) : base(context)
    {

    }
}
