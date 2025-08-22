using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace RentACar.Persistence.Services;

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepo;

    public CarService(IRepository<Car> carRepo)
    {
        _carRepo = carRepo;
    }

    public async Task<BaseResponse<CarGetDto>> CreateAsync(CarCreateDto dto)
    {
        var entity = new Car
        {
            Id = Guid.NewGuid(),
            OwnerId = dto.OwnerId,
            CompanyId = dto.CompanyId,
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

        // Əlavə olaraq Feature və Image-ləri bağlamaq olar
        if (dto.FeatureIds is not null && dto.FeatureIds.Any())
        {
            entity.Features = dto.FeatureIds
                .Select(fid => new CarFeatureAssignment { CarId = entity.Id, FeatureId = fid })
                .ToList();
        }

        if (dto.ImageUrls is not null && dto.ImageUrls.Any())
        {
            entity.Images = dto.ImageUrls
                .Select(url => new CarImage { CarId = entity.Id, ImageUrl = url })
                .ToList();
        }

        await _carRepo.AddAsync(entity);
        await _carRepo.SaveChangeAsync();

        var result = MapToGetDto(entity);

        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<CarGetDto>> GetByIdAsync(Guid id)
    {
        var entity = await _carRepo.GetByFiltered(
            x => x.Id == id,
            include: new Expression<Func<Car, object>>[]
            {
                x => x.Owner,
                x => x.Company,
                x => x.Features,
                x => x.Images
            },
            IsTracking: false
        ).FirstOrDefaultAsync();

        if (entity is null)
            return BaseResponse<CarGetDto>.FailResponse("Car not found", HttpStatusCode.NotFound);

        var result = MapToGetDto(entity);
        return BaseResponse<CarGetDto>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<CarGetDto>>> GetAllAsync()
    {
        var entities = await _carRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Company)
            .Include(x => x.Features)
            .Include(x => x.Images)
            .ToListAsync();

        var result = entities.Select(MapToGetDto);
        return BaseResponse<IEnumerable<CarGetDto>>.SuccessResponse(result, "Cars retrieved successfully");
    }

    public async Task<BaseResponse<CarGetDto>> UpdateAsync(Guid id, CarUpdateDto dto)
    {
        var entity = await _carRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<CarGetDto>.FailResponse("Car not found", HttpStatusCode.NotFound);

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
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car updated successfully");
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _carRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<bool>.FailResponse("Car not found", HttpStatusCode.NotFound);

        _carRepo.Delete(entity);
        await _carRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Car deleted successfully");
    }

    // 🔑 Mapping helper
    private CarGetDto MapToGetDto(Car e)
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
            query = query.Where(x => x.Brand.Contains(filter.Brand));

        if (!string.IsNullOrWhiteSpace(filter.Model))
            query = query.Where(x => x.Model.Contains(filter.Model));

        if (filter.Year.HasValue)
            query = query.Where(x => x.Year == filter.Year);

        if (filter.TransmissionType.HasValue)
            query = query.Where(x => x.TransmissionType == filter.TransmissionType);

        if (filter.FuelType.HasValue)
            query = query.Where(x => x.FuelType == filter.FuelType);

        if (filter.MinPrice.HasValue)
            query = query.Where(x => x.DailyPrice >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(x => x.DailyPrice <= filter.MaxPrice);

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(x => x.Location.Contains(filter.Location));

        if (filter.IsApproved.HasValue)
            query = query.Where(x => x.IsApproved == filter.IsApproved);

        var entities = await query.ToListAsync();

        var result = entities.Select(MapToGetDto);

        return BaseResponse<IEnumerable<CarGetDto>>.SuccessResponse(result, "Filtered cars retrieved successfully");
    }
}
