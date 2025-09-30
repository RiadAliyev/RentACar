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
    Task<BaseResponse<IEnumerable<BookingGetDto>>> GetPendingForOwnerAsync();
    Task<BaseResponse<string>> ApproveAsync(Guid id);
    Task<BaseResponse<string>> RejectAsync(Guid id);
    Task<BaseResponse<string>> CancelAsync(Guid id, bool byAdmin = false);

    Task<BaseResponse<string>> CompleteAsync(Guid id);
}
