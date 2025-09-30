using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.PaymentDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Persistence.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Booking> _bookingRepository;

    public PaymentService(IRepository<Payment> paymentRepository, IRepository<Booking> bookingRepository)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<BaseResponse<PaymentGetDto>> CreateAsync(PaymentCreateDto dto)
    {
        var booking = await _bookingRepository.GetByIdAsync(dto.BookingId);
        if (booking is null)
            return BaseResponse<PaymentGetDto>.FailResponse("Booking not found", HttpStatusCode.NotFound);

        var payment = new Payment
        {
            BookingId = dto.BookingId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            IsSuccessful = dto.IsSuccessful,
            PaidAt = dto.PaidAt
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangeAsync();

        var result = MapToGetDto(payment, booking.TotalPrice);
        return BaseResponse<PaymentGetDto>.SuccessResponse(result, "Payment created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<PaymentGetDto>> UpdateAsync(Guid id, PaymentUpdateDto dto)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment is null)
            return BaseResponse<PaymentGetDto>.FailResponse("Payment not found", HttpStatusCode.NotFound);

        payment.Amount = dto.Amount;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.IsSuccessful = dto.IsSuccessful;
        payment.PaidAt = dto.PaidAt;

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangeAsync();

        var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
        var result = MapToGetDto(payment, booking?.TotalPrice ?? 0);

        return BaseResponse<PaymentGetDto>.SuccessResponse(result, "Payment updated successfully");
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment is null)
            return BaseResponse<bool>.FailResponse("Payment not found", HttpStatusCode.NotFound);

        _paymentRepository.Delete(payment);
        await _paymentRepository.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Payment deleted successfully");
    }

    public async Task<BaseResponse<PaymentGetDto>> GetByIdAsync(Guid id)
    {
        var payment = await _paymentRepository
            .GetByFiltered(x => x.Id == id, include: new[] { (Expression<Func<Payment, object>>)(p => p.Booking) })
            .FirstOrDefaultAsync();

        if (payment is null)
            return BaseResponse<PaymentGetDto>.FailResponse("Payment not found", HttpStatusCode.NotFound);

        var result = MapToGetDto(payment, payment.Booking.TotalPrice);
        return BaseResponse<PaymentGetDto>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<PaymentGetDto>>> GetAllAsync()
    {
        var payments = await _paymentRepository
            .GetAllFiltered(include: new[] { (Expression<Func<Payment, object>>)(p => p.Booking) })
            .ToListAsync();

        var result = payments.Select(p => MapToGetDto(p, p.Booking.TotalPrice)).ToList();
        return BaseResponse<IEnumerable<PaymentGetDto>>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<PaymentGetDto>>> GetByBookingIdAsync(Guid bookingId)
    {
        var payments = await _paymentRepository
            .GetByFiltered(p => p.BookingId == bookingId, include: new[] { (Expression<Func<Payment, object>>)(p => p.Booking) })
            .ToListAsync();

        var result = payments.Select(p => MapToGetDto(p, p.Booking.TotalPrice)).ToList();
        return BaseResponse<IEnumerable<PaymentGetDto>>.SuccessResponse(result);
    }

    private static PaymentGetDto MapToGetDto(Payment payment, decimal bookingTotalPrice)
    {
        return new PaymentGetDto
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod.ToString(),
            IsSuccessful = payment.IsSuccessful,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt,
            BookingTotalPrice = bookingTotalPrice
        };
    }

}
