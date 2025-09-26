using Microsoft.EntityFrameworkCore;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Persistence.Contexts;
using RentACar.Persistence;
using RentACar.Persistence.Repositories;
using FluentValidation;
using RentACar.Application.Validations;
using FluentValidation.AspNetCore;
using RentACar.Application.Shared.Settings;
using RentACar.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using RentACar.Application.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Swagger üçün vacibdir
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Rent_A_Car API",
        Version = "v1"
    });

    // JWT üçün təhlükəsizlik sxemini əlavə et
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT token daxil edin. Format: Bearer {token}"
    });

    // Təhlükəsizlik tələbi əlavə olunur
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme 
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddDbContext<RentACarDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    //options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
}).AddEntityFrameworkStores<RentACarDbContext>()
  .AddDefaultTokenProviders();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));  //cluidynary fileupload ucun


builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JWTSettings>();


builder.Services.AddAuthorization(options =>
{
    foreach (var permission in PermissionHelper.GetAllPermissionList())
    {
        options.AddPolicy(permission, policy =>
        {
            policy.RequireClaim("Permission", permission);
        });
    }
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };

    //// 🔥 BURADA BLACKLIST CHECK EDİLİR
    //options.Events = new JwtBearerEvents
    //{
    //    OnTokenValidated = async context =>
    //    {
    //        var redisService = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
    //        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    //        var isBlacklisted = await redisService.GetAsync<bool>($"blacklist:{token}");
    //        if (isBlacklisted)
    //        {
    //            context.Fail("Token is blacklisted");
    //        }
    //    }
    //};
});



builder.Services.AddValidatorsFromAssembly(typeof(UserRegisterDtoValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));//service registration
builder.Services.RegisterService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
