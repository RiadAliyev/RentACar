using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace RentACar.Persistence.Services;

public class CarImageService : ICarImageService
{
    private readonly IRepository<CarImage> _carImageRepo;

    public CarImageService(IRepository<CarImage> carImageRepo)
    {
        _carImageRepo = carImageRepo;
    }

    public async Task<BaseResponse<CarImageGetDto>> CreateAsync(CarImageCreateDto dto)
    {
        var entity = new CarImage
        {
            Id = Guid.NewGuid(),
            CarId = dto.CarId,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _carImageRepo.AddAsync(entity);
        await _carImageRepo.SaveChangeAsync();

        var result = new CarImageGetDto
        {
            Id = entity.Id,
            CarId = entity.CarId,
            ImageUrl = entity.ImageUrl,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarImageGetDto>.SuccessResponse(result, "Car image created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<CarImageGetDto>> GetByIdAsync(Guid id)
    {
        var entity = await _carImageRepo.GetByFiltered(
            x => x.Id == id,
            include: new Expression<Func<CarImage, object>>[] { x => x.Car },
            IsTracking: false
        ).FirstOrDefaultAsync();

        if (entity is null)
            return BaseResponse<CarImageGetDto>.FailResponse("Car image not found", HttpStatusCode.NotFound);

        var result = new CarImageGetDto
        {
            Id = entity.Id,
            CarId = entity.CarId,
            ImageUrl = entity.ImageUrl,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarImageGetDto>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<CarImageGetDto>>> GetAllAsync()
    {
        var entities = await _carImageRepo.GetAll(IsTracking: false)
                                          .Include(x => x.Car)
                                          .ToListAsync();

        var result = entities.Select(e => new CarImageGetDto
        {
            Id = e.Id,
            CarId = e.CarId,
            ImageUrl = e.ImageUrl,
            CreatedAt = e.CreatedAt
        });

        return BaseResponse<IEnumerable<CarImageGetDto>>.SuccessResponse(result, "Car images retrieved successfully");
    }

    public async Task<BaseResponse<CarImageGetDto>> UpdateAsync(Guid id, CarImageUpdateDto dto)
    {
        var entity = await _carImageRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<CarImageGetDto>.FailResponse("Car image not found", HttpStatusCode.NotFound);

        entity.ImageUrl = dto.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;

        _carImageRepo.Update(entity);
        await _carImageRepo.SaveChangeAsync();

        var result = new CarImageGetDto
        {
            Id = entity.Id,
            CarId = entity.CarId,
            ImageUrl = entity.ImageUrl,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarImageGetDto>.SuccessResponse(result, "Car image updated successfully");
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _carImageRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<bool>.FailResponse("Car image not found", HttpStatusCode.NotFound);

        _carImageRepo.Delete(entity);
        await _carImageRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Car image deleted successfully");
    }
}
