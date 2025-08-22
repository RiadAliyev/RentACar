using Microsoft.EntityFrameworkCore;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.BookingDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using System.Net;

namespace RentACar.Persistence.Services;

public class BookingService : IBookingService
{
    private readonly IRepository<Booking> _bookingRepository;

    public BookingService(IRepository<Booking> bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BaseResponse<BookingGetDto>> GetByIdAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByFiltered(
            x => x.Id == id,
            include: new Expression<Func<Booking, object>>[]
            {
            x => x.Car,
            x => x.Customer
            },
            IsTracking: false
        ).FirstOrDefaultAsync();

        if (booking is null)
            return new BaseResponse<BookingGetDto>(
                message: "Booking not found",
                statusCode: HttpStatusCode.NotFound
            );

        var result = new BookingGetDto
        {
            Id = booking.Id,
            CarId = booking.CarId,
            CarBrand = booking.Car.Brand,
            CarModel = booking.Car.Model,
            CustomerId = booking.CustomerId,
            CustomerName = booking.Customer.FullName,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice,
            DepositAmount = booking.DepositAmount,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };

        return new BaseResponse<BookingGetDto>(
            message: "Booking retrieved successfully",
            data: result,
            statusCode: HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<IEnumerable<BookingGetDto>>> GetAllAsync()
    {
        var bookings = await _bookingRepository
            .GetAll(true)                 // IQueryable<Booking>
            .Include(x => x.Car)
            .Include(x => x.Customer)
            .ToListAsync();


        var result = bookings.Select(b => new BookingGetDto
        {
            Id = b.Id,
            CarId = b.CarId,
            CarBrand = b.Car.Brand,
            CarModel = b.Car.Model,
            CustomerId = b.CustomerId,
            CustomerName = b.Customer.FullName,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalPrice = b.TotalPrice,
            DepositAmount = b.DepositAmount,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });

        // ✅ Burada message verilir (BaseResponse constructor tələb edir)
        return new BaseResponse<IEnumerable<BookingGetDto>>(
            message: "Bookings retrieved successfully",
            data: result,
            statusCode: HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<string>> CreateAsync(BookingCreateDto dto)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            CarId = dto.CarId,
            CustomerId = dto.CustomerId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalPrice = dto.TotalPrice,
            DepositAmount = dto.DepositAmount,
            Status = dto.Status
        };

        await _bookingRepository.AddAsync(booking);
        await _bookingRepository.SaveChangeAsync();

        return new BaseResponse<string>(
            message: "Booking created successfully",
            data: booking.Id.ToString(), // string olaraq ID qaytarıram
            statusCode: HttpStatusCode.Created
        );
    }


    public async Task<BaseResponse<string>> UpdateAsync(Guid id, BookingUpdateDto dto)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null)
        {
            return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);
        }

        booking.StartDate = dto.StartDate;
        booking.EndDate = dto.EndDate;
        booking.TotalPrice = dto.TotalPrice;
        booking.DepositAmount = dto.DepositAmount;
        booking.Status = dto.Status;

        _bookingRepository.Update(booking);
        await _bookingRepository.SaveChangeAsync();

        return new BaseResponse<string>(
            message: "Booking updated successfully",
            data: booking.Id.ToString(),
            statusCode: HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null)
            return new BaseResponse<string>("Booking not found", HttpStatusCode.NotFound);

        _bookingRepository.Delete(booking);
        await _bookingRepository.SaveChangeAsync();

        return new BaseResponse<string>("Booking deleted successfully", HttpStatusCode.OK);
    }
}
