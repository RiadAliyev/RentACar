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

namespace RentACar.Persistence.Services;

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepo;
    private readonly IRepository<CarImage> _imgRepo; 
    private readonly IFileStorage _files;

    public CarService(IRepository<Car> carRepo, IRepository<CarImage> imgRepo, IFileStorage files )
    {
        _carRepo = carRepo;
        _imgRepo = imgRepo;
        _files = files;
    }

    public async Task<BaseResponse<CarGetDto>> CreateAsync(CarCreateDto dto)
    {
        var car = new Car
        {
            Id = Guid.NewGuid(),
            Brand = dto.Brand,          // <- səndə necədirsə
            Model = dto.Model,          // <- səndə necədirsə
            Year = dto.Year,           // <- səndə necədirsə
            DailyPrice = dto.DailyPrice,     // <- səndə necədirsə
            OwnerId = dto.OwnerId,     // (əgər varsa / lazım olsa)
            CompanyId = dto.CompanyId ,    // (əgər varsa / lazım olsa)
            CreatedAt = DateTime.UtcNow
        };

        await _carRepo.AddAsync(car);
        await _carRepo.SaveChangeAsync();

        // 2) Şəkilləri yüklə və CarImage kimi əlavə et
        var createdImages = new List<CarImage>();
        if (dto.Images is { Count: > 0 })
        {
            foreach (var file in dto.Images)
            {
                if (file == null || file.Length == 0) continue;

                // Faylı diskinə yaz və relative URL al
                var url = await _files.SaveCarImageAsync(file, car.Id);

                var img = new CarImage
                {
                    Id = Guid.NewGuid(),
                    CarId = car.Id,
                    ImageUrl = url,
                    CreatedAt = DateTime.UtcNow
                };

                await _imgRepo.AddAsync(img);
                createdImages.Add(img);
            }

            await _imgRepo.SaveChangeAsync();
        }

        // 3) Nəticə DTO
        var result = new CarGetDto
        {
            Id = car.Id,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            DailyPrice = car.DailyPrice,
            // başqa sahələrin varsa əlavə et...
            Images = createdImages.Select(i => new CarImageGetDto
            {
                Id = i.Id,
                CarId = i.CarId,
                ImageUrl = i.ImageUrl,
                CreatedAt = i.CreatedAt
            }).ToList()
        };

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
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car retrieved successfully", HttpStatusCode.OK);
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
        return BaseResponse<IEnumerable<CarGetDto>>.SuccessResponse(result, "Cars retrieved successfully", HttpStatusCode.OK);
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
        return BaseResponse<CarGetDto>.SuccessResponse(result, "Car updated successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _carRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<bool>.FailResponse("Car not found", HttpStatusCode.NotFound);

        _carRepo.Delete(entity);
        await _carRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Car deleted successfully", HttpStatusCode.OK);
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

        return BaseResponse<IEnumerable<CarGetDto>>.SuccessResponse(result, "Filtered cars retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<CarImageGetDto>>> AddImagesAsync(Guid carId, IEnumerable<IFormFile> files)
    {
        // 0) Input yoxlaması
        if (files is null || !files.Any())
            return BaseResponse<IEnumerable<CarImageGetDto>>.FailResponse("No files provided", HttpStatusCode.BadRequest);

        // 1) Maşını yoxla
        var car = await _carRepo.GetByIdAsync(carId);
        if (car is null)
            return BaseResponse<IEnumerable<CarImageGetDto>>.FailResponse("Car not found", HttpStatusCode.NotFound);

        // 2) Yüklə və DB-yə yaz
        var created = new List<CarImageGetDto>();
        foreach (var file in files)
        {
            if (file == null || file.Length == 0) continue; // boş faylı keç

            var url = await _files.SaveCarImageAsync(file, car.Id); // <<-- 'file' istifadə olunur
            var img = new CarImage
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

        // Köhnə fiziki faylı sil
        _files.DeleteIfExists(img.ImageUrl);

        // Yenini yaddaşa yaz
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

        // Fiziki faylı sil
        _files.DeleteIfExists(img.ImageUrl);

        _imgRepo.Delete(img);
        await _imgRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Image deleted");
    }
}
