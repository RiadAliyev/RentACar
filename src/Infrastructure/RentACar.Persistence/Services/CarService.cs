using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;
using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared.Helpers;
using static RentACar.Application.Shared.Permissions;
using Microsoft.AspNetCore.Identity;
using RentACar.Domain.Enums;

namespace RentACar.Persistence.Services;

public class CarService : ICarService
{
    private readonly IRepository<RentACar.Domain.Entities.Car> _carRepo;
    private readonly IRepository<RentACar.Domain.Entities.CarImage> _imgRepo; 
    private readonly IFileStorage _files;
    private readonly IRepository<CarFeatureAssignment> _carFeatureAssignmentRepo;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _userManager;
    private string? CurrentUserId =>
    _http.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    public CarService(IRepository<Domain.Entities.Car> carRepo, 
        IRepository<Domain.Entities.CarImage> imgRepo, 
        IFileStorage files, 
        IRepository<CarFeatureAssignment> carFeatureAssignmentRepo, 
        IHttpContextAccessor http,
        UserManager<AppUser> userManager)
    {
        _carRepo = carRepo;
        _imgRepo = imgRepo;
        _files = files;
        _carFeatureAssignmentRepo = carFeatureAssignmentRepo;
        _http = http;
        _userManager = userManager;

    }

    public async Task<BaseResponse<CarGetDto>> CreateAsync(CarCreateDto dto)
    {
       
        if (string.IsNullOrWhiteSpace(CurrentUserId))
            return BaseResponse<CarGetDto>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        var userId = Guid.Parse(CurrentUserId);
        var user = await _userManager.Users
            .Include(u => u.Company) 
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return BaseResponse<CarGetDto>.FailResponse("User not found", HttpStatusCode.NotFound);

      
        Guid? enforcedOwnerId = null;
        Guid? enforcedCompanyId = null;

        switch (user.AccountType)
        {
            case AccountType.CarOwner:
                
                enforcedOwnerId = user.Id;
                enforcedCompanyId = null;
                break;

            case AccountType.CompanyOwner:
                
                if (user.Company is null)
                    return BaseResponse<CarGetDto>.FailResponse("You must create/link a company before adding cars.", HttpStatusCode.BadRequest);

                enforcedOwnerId = null;
                enforcedCompanyId = user.Company.Id; 
                break;

            default:
                
                return BaseResponse<CarGetDto>.FailResponse("You are not allowed to create cars.", HttpStatusCode.Forbidden);
        }

        
        var car = new RentACar.Domain.Entities.Car
        {
            Id = Guid.NewGuid(),
            OwnerId = enforcedOwnerId,      
            CompanyId = enforcedCompanyId,    
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            TransmissionType = dto.TransmissionType,
            FuelType = dto.FuelType,
            Seats = dto.Seats,
            DailyPrice = dto.DailyPrice,
            Location = dto.Location,
            IsApproved = dto.IsApproved,
            CreatedAt = DateTime.UtcNow
        };

        await _carRepo.AddAsync(car);
        await _carRepo.SaveChangeAsync();

        
        if (dto.Images is { Count: > 0 })
        {
            foreach (var file in dto.Images)
            {
                if (file == null || file.Length == 0) continue;

                var url = await _files.SaveCarImageAsync(file, car.Id);
                var img = new RentACar.Domain.Entities.CarImage
                {
                    Id = Guid.NewGuid(),
                    CarId = car.Id,
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow
                };
                await _imgRepo.AddAsync(img);
            }
            await _imgRepo.SaveChangeAsync();
        }

      
        if (dto.FeatureIds is { Count: > 0 })
        {
            foreach (var featureId in dto.FeatureIds)
            {
                var cf = new CarFeatureAssignment
                {
                    Id = Guid.NewGuid(),
                    CarId = car.Id,
                    FeatureId = featureId,
                    CreatedAt = DateTime.UtcNow
                };
                await _carFeatureAssignmentRepo.AddAsync(cf);
            }
            await _carFeatureAssignmentRepo.SaveChangeAsync();
        }

        
        var created = await _carRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Company)
            .Include(x => x.Images)
            .Include(x => x.Features).ThenInclude(cf => cf.Feature)
            .FirstAsync(x => x.Id == car.Id);

        var result = MapToGetDto(created);
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<CarGetDto>> GetByIdAsync(Guid id)
    {
        var entity = await _carRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Company)
            .Include(x => x.Images)
            .Include(x => x.Features).ThenInclude(cf => cf.Feature) 
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
            return BaseResponse<CarGetDto>.FailResponse("Car not found", HttpStatusCode.NotFound);

        var result = MapToGetDto(entity);
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<CarGetDto>>> GetAllAsync()
    {
        var entities = await _carRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Company)
            .Include(x => x.Images)
            .Include(x => x.Features).ThenInclude(cf => cf.Feature) 
            .ToListAsync();

        var result = entities.Select(MapToGetDto);
        return BaseResponse<IEnumerable<CarGetDto>>.SuccessResponse(result, "Cars retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CarGetDto>> UpdateAsync(Guid id, CarUpdateDto dto)
    {
        var entity = await _carRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<CarGetDto>.FailResponse("Car not found", HttpStatusCode.NotFound);

       
        if (entity.OwnerId.ToString() != CurrentUserId)
        {
            return BaseResponse<CarGetDto>.FailResponse("You can only update your own cars", HttpStatusCode.Forbidden);
        }

        entity.CompanyId = dto.CompanyId;
        entity.Brand = dto.Brand;
        entity.Model = dto.Model;
        entity.Year = dto.Year;
        entity.TransmissionType = dto.TransmissionType;
        entity.FuelType = dto.FuelType;
        entity.Seats = dto.Seats;
        entity.DailyPrice = dto.DailyPrice;
        entity.Location = dto.Location;
        entity.IsApproved = dto.IsApproved;
        entity.UpdatedAt = DateTime.UtcNow;

        _carRepo.Update(entity);
        await _carRepo.SaveChangeAsync();

        var result = MapToGetDto(entity);
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car updated successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _carRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<bool>.FailResponse("Car not found", HttpStatusCode.NotFound);

        
        if (entity.OwnerId.ToString() != CurrentUserId)
        {
            return BaseResponse<bool>.FailResponse("You can only delete your own cars", HttpStatusCode.Forbidden);
        }

        _carRepo.Delete(entity);
        await _carRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Car deleted successfully", HttpStatusCode.OK);
    }

  
    private CarGetDto MapToGetDto(RentACar.Domain.Entities.Car e)
    {
        return new CarGetDto
        {
            Id = e.Id,
            OwnerId = e.OwnerId,
            OwnerName = e.Owner?.FullName,
            CompanyId = e.CompanyId,
            CompanyName = e.Company?.Name,
            Brand = e.Brand,
            Model = e.Model,
            Year = e.Year,
            TransmissionType = e.TransmissionType,
            FuelType = e.FuelType,
            Seats = e.Seats,
            DailyPrice = e.DailyPrice,
            Location = e.Location,
            IsApproved = e.IsApproved,
            CreatedAt = e.CreatedAt,
            Features = e.Features?.Select(f => f.Feature.Name).ToList() ?? new(),
            ImageUrls = e.Images?.Select(img => img.ImageUrl).ToList() ?? new()
        };
    }

    public async Task<BaseResponse<IEnumerable<CarGetDto>>> GetByFilterAsync(CarFilterDto filter)
    {
        var query = _carRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Company)
            .Include(x => x.Features).ThenInclude(f => f.Feature)
            .Include(x => x.Images)
            .AsQueryable();

      
        if (!string.IsNullOrWhiteSpace(filter.Brand))
            query = query.Where(x => EF.Functions.Like(x.Brand, $"%{filter.Brand.Trim()}%"));

        if (!string.IsNullOrWhiteSpace(filter.Model))
            query = query.Where(x => EF.Functions.Like(x.Model, $"%{filter.Model.Trim()}%"));

        if (filter.Year.HasValue)
            query = query.Where(x => x.Year == filter.Year.Value);

        if (filter.TransmissionType.HasValue)
            query = query.Where(x => x.TransmissionType == filter.TransmissionType);

        if (filter.FuelType.HasValue)
            query = query.Where(x => x.FuelType == filter.FuelType);

        if (filter.MinPrice.HasValue)
            query = query.Where(x => x.DailyPrice >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(x => x.DailyPrice <= filter.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(x => EF.Functions.Like(x.Location, $"%{filter.Location.Trim()}%"));

        

        
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var tokens = filter.Search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var t in tokens)
            {
                var term = t; 
                              
                var isYear = short.TryParse(term, out var yearVal);

                query = query.Where(x =>
                    EF.Functions.Like(x.Brand, $"%{term}%") ||
                    EF.Functions.Like(x.Model, $"%{term}%") ||
                    EF.Functions.Like(x.Location, $"%{term}%") ||
                    (isYear && x.Year == yearVal)
                );
            }
        }

        
        query = query.OrderByDescending(x => x.CreatedAt);

        var entities = await query.ToListAsync();
        var result = entities.Select(MapToGetDto);

        return BaseResponse<IEnumerable<CarGetDto>>
            .SuccessResponse(result, "Filtered cars retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<CarImageGetDto>>> AddImagesAsync(Guid carId, IEnumerable<IFormFile> files)
    {
        if (files is null || !files.Any())
            return BaseResponse<IEnumerable<CarImageGetDto>>.FailResponse("No files provided", HttpStatusCode.BadRequest);

        var car = await _carRepo.GetByIdAsync(carId);
        if (car is null)
            return BaseResponse<IEnumerable<CarImageGetDto>>.FailResponse("Car not found", HttpStatusCode.NotFound);

        
        if (car.OwnerId.ToString() != CurrentUserId)
            return BaseResponse<IEnumerable<CarImageGetDto>>.FailResponse("You can only add images to your own cars", HttpStatusCode.Forbidden);

        var created = new List<CarImageGetDto>();
        foreach (var file in files)
        {
            if (file == null || file.Length == 0) continue;

            var url = await _files.SaveCarImageAsync(file, car.Id);
            var img = new RentACar.Domain.Entities.CarImage
            {
                Id = Guid.NewGuid(),
                CarId = carId,
                ImageUrl = url,
                CreatedAt = DateTime.UtcNow
            };

            await _imgRepo.AddAsync(img);

            created.Add(new CarImageGetDto
            {
                Id = img.Id,
                CarId = img.CarId,
                ImageUrl = img.ImageUrl,
                CreatedAt = img.CreatedAt
            });
        }

        await _imgRepo.SaveChangeAsync();

        return BaseResponse<IEnumerable<CarImageGetDto>>.SuccessResponse(created, "Images uploaded");
    }


    public async Task<BaseResponse<CarImageGetDto>> ReplaceImageAsync(Guid imageId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BaseResponse<CarImageGetDto>.FailResponse("File is required", HttpStatusCode.BadRequest);

        var img = await _imgRepo.GetByIdAsync(imageId);
        if (img is null)
            return BaseResponse<CarImageGetDto>.FailResponse("Image not found", HttpStatusCode.NotFound);

        var car = await _carRepo.GetByIdAsync(img.CarId);
        if (car == null)
            return BaseResponse<CarImageGetDto>.FailResponse("Car not found", HttpStatusCode.NotFound);

        
        if (car.OwnerId.ToString() != CurrentUserId)
            return BaseResponse<CarImageGetDto>.FailResponse("You can only update images of your own cars", HttpStatusCode.Forbidden);

        _files.DeleteIfExists(img.ImageUrl);

        var url = await _files.SaveCarImageAsync(file, img.CarId);
        img.ImageUrl = url;
        img.UpdatedAt = DateTime.UtcNow;

        _imgRepo.Update(img);
        await _imgRepo.SaveChangeAsync();

        var dto = new CarImageGetDto
        {
            Id = img.Id,
            CarId = img.CarId,
            ImageUrl = img.ImageUrl,
            CreatedAt = img.CreatedAt
        };

        return BaseResponse<CarImageGetDto>.SuccessResponse(dto, "Image replaced");
    }


    public async Task<BaseResponse<bool>> DeleteImageAsync(Guid imageId)
    {
        var img = await _imgRepo.GetByIdAsync(imageId);
        if (img is null)
            return BaseResponse<bool>.FailResponse("Image not found", HttpStatusCode.NotFound);

        var car = await _carRepo.GetByIdAsync(img.CarId);
        if (car == null)
            return BaseResponse<bool>.FailResponse("Car not found", HttpStatusCode.NotFound);

        
        if (car.OwnerId.ToString() != CurrentUserId)
            return BaseResponse<bool>.FailResponse("You can only delete images of your own cars", HttpStatusCode.Forbidden);

        _files.DeleteIfExists(img.ImageUrl);

        _imgRepo.Delete(img);
        await _imgRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Image deleted");
    }
}
