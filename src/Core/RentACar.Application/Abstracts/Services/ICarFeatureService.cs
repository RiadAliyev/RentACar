using RentACar.Application.DTOs.CarFeatureDto;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface ICarFeatureService
{
    Task<BaseResponse<CarFeatureGetDto>> CreateAsync(CarFeatureCreateDto dto);
    Task<BaseResponse<CarFeatureGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<CarFeatureGetDto>>> GetAllAsync();
    Task<BaseResponse<CarFeatureGetDto>> UpdateAsync(Guid id, CarFeatureUpdateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
}
