using Microsoft.AspNetCore.Http;
using RentACar.Application.DTOs.CarDtos;
using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;
public interface ICarService
{
    Task<BaseResponse<CarGetDto>> CreateAsync(CarCreateDto dto);
    Task<BaseResponse<CarGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<CarGetDto>>> GetAllAsync();
    Task<BaseResponse<CarGetDto>> UpdateAsync(Guid id, CarUpdateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<IEnumerable<CarGetDto>>> GetByFilterAsync(CarFilterDto filter);
    Task<BaseResponse<IEnumerable<CarImageGetDto>>> AddImagesAsync(Guid carId, IEnumerable<IFormFile> files);
    Task<BaseResponse<CarImageGetDto>> ReplaceImageAsync(Guid imageId, IFormFile file);
    Task<BaseResponse<bool>> DeleteImageAsync(Guid imageId);
}
