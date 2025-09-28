using RentACar.Application.DTOs.ReviewDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IReviewService
{
    Task<BaseResponse<ReviewGetDto>> CreateAsync(ReviewCreateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<IEnumerable<ReviewGetDto>>> GetAllByCarIdAsync(Guid carId);
}
