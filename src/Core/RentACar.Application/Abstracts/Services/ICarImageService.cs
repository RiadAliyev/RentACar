using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface ICarImageService
{
    Task<BaseResponse<CarImageGetDto>> CreateAsync(CarImageCreateDto dto);
    Task<BaseResponse<CarImageGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<CarImageGetDto>>> GetAllAsync();
    Task<BaseResponse<CarImageGetDto>> UpdateAsync(Guid id, CarImageUpdateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
