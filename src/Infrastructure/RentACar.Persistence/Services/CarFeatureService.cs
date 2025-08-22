using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarFeatureDto;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Persistence.Services;

public class CarFeatureService : ICarFeatureService
{
    private readonly IRepository<CarFeature> _carFeatureRepo;

    public CarFeatureService(IRepository<CarFeature> carFeatureRepo)
    {
        _carFeatureRepo = carFeatureRepo;
    }

    public async Task<BaseResponse<CarFeatureGetDto>> CreateAsync(CarFeatureCreateDto dto)
    {
        var entity = new CarFeature
        {
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _carFeatureRepo.AddAsync(entity);
        await _carFeatureRepo.SaveChangeAsync();

        var result = new CarFeatureGetDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarFeatureGetDto>.SuccessResponse(result, "Car feature created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<CarFeatureGetDto>> GetByIdAsync(Guid id)
    {
        var entity = await _carFeatureRepo.GetByIdAsync(id);

        if (entity is null)
            return BaseResponse<CarFeatureGetDto>.FailResponse("Car feature not found", HttpStatusCode.NotFound);

        var result = new CarFeatureGetDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarFeatureGetDto>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<CarFeatureGetDto>>> GetAllAsync()
    {
        var entities = await _carFeatureRepo.GetAll(IsTracking: false).ToListAsync();

        var result = entities.Select(e => new CarFeatureGetDto
        {
            Id = e.Id,
            Name = e.Name,
            CreatedAt = e.CreatedAt
        });

        return BaseResponse<IEnumerable<CarFeatureGetDto>>.SuccessResponse(result);
    }

    public async Task<BaseResponse<CarFeatureGetDto>> UpdateAsync(Guid id, CarFeatureUpdateDto dto)
    {
        var entity = await _carFeatureRepo.GetByIdAsync(id);

        if (entity is null)
            return BaseResponse<CarFeatureGetDto>.FailResponse("Car feature not found", HttpStatusCode.NotFound);

        entity.Name = dto.Name;
        entity.UpdatedAt = DateTime.UtcNow;

        _carFeatureRepo.Update(entity);
        await _carFeatureRepo.SaveChangeAsync();

        var result = new CarFeatureGetDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt
        };

        return BaseResponse<CarFeatureGetDto>.SuccessResponse(result, "Car feature updated successfully");
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _carFeatureRepo.GetByIdAsync(id);

        if (entity is null)
            return BaseResponse<bool>.FailResponse("Car feature not found", HttpStatusCode.NotFound);

        _carFeatureRepo.Delete(entity);
        await _carFeatureRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Car feature deleted successfully");
    }
}
