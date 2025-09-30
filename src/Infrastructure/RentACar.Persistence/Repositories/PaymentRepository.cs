using RentACar.Application.Abstracts.Repositories;
using RentACar.Domain.Entities;
using RentACar.Persistence.Contexts;

namespace RentACar.Persistence.Repositories;

public class PaymentRepository:Repository<Payment>,IPaymentRepository
{
    public PaymentRepository(RentACarDbContext context) : base(context)
    {

    }
}
