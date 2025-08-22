using RentACar.Application.DTOs.PaymentDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IPaymentService
{
    Task<BaseResponse<PaymentGetDto>> CreateAsync(PaymentCreateDto dto);
    Task<BaseResponse<PaymentGetDto>> UpdateAsync(Guid id, PaymentUpdateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);

    Task<BaseResponse<PaymentGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<PaymentGetDto>>> GetAllAsync();
    Task<BaseResponse<IEnumerable<PaymentGetDto>>> GetByBookingIdAsync(Guid bookingId);
}
