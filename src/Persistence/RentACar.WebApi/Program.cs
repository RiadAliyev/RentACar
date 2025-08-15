using Microsoft.EntityFrameworkCore;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Persistence.Contexts;
using RentACar.Persistence;
using RentACar.Persistence.Repositories;
using FluentValidation;
using RentACar.Application.Validations;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RentACarDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddValidatorsFromAssembly(typeof(UserCreateDtoValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));//service registration
builder.Services.RegisterService();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
