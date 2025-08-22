using RentACar.Application.DTOs.CarDtos;
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
}
