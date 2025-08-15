using Microsoft.Extensions.DependencyInjection;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Persistence.Repositories;

namespace RentACar.Persistence;

public static class ServiceRegistration
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repositories
        services.AddScoped<ICarImageRepository, CarImageRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ICarFeatureRepository, CarFeatureRepository>();
        services.AddScoped<ICarsRepository, CarsRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        #endregion

        #region Services




        #endregion

    }
}
