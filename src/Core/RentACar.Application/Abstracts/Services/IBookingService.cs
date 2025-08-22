using RentACar.Application.DTOs.BookingDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IBookingService
{
    Task<BaseResponse<BookingGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<BookingGetDto>>> GetAllAsync();
    Task<BaseResponse<string>> CreateAsync(BookingCreateDto dto);
    Task<BaseResponse<string>> UpdateAsync(Guid id, BookingUpdateDto dto);
    Task<BaseResponse<string>> DeleteAsync(Guid id);
}
