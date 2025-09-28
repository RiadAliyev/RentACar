using Microsoft.Extensions.DependencyInjection;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Infrastructure.Services;
using RentACar.Persistence.Repositories;
using RentACar.Persistence.Services;

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

        #endregion

        #region Services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ICarFeatureService, CarFeatureService>();
        services.AddScoped<IFileService, CloudinaryService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
    





        #endregion

    }
}
