using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.BookingDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using RentACar.Domain.Enums;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;

namespace RentACar.Persistence.Services;

public class BookingService : IBookingService
{
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IRepository<Car> _carRepo;
    private readonly IHttpContextAccessor _http;

    
    private const decimal DepositPercent = 0.20m;

    public BookingService(
        IRepository<Booking> bookingRepo,
        IRepository<Car> carRepo,
        IHttpContextAccessor http)
    {
        _bookingRepo = bookingRepo;
        _carRepo = carRepo;
        _http = http;
    }

    
    private Guid? CurrentUserId
    {
        get
        {
            var id = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var g) ? g : null;
        }
    }

    private static DateTime StartOfDay(DateOnly d) => d.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
    private static DateTime EndOfDay(DateOnly d) => d.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);

    private static int CountDays(DateOnly start, DateOnly end)
    {
        // Gün sayını düzgün hesablamaq üçün (start daxil, end xaric) – kirayələrdə standart budur
        // Əgər səndə hər iki gün daxil olmalıdırsa, +1 elə.
        var days = end.DayNumber - start.DayNumber; // end > start qəbul edirik
        return days <= 0 ? 1 : days;
    }

   

    public async Task<BaseResponse<BookingGetDto>> GetByIdAsync(Guid id)
    {
        var booking = await _bookingRepo
            .GetByFiltered(b => b.Id == id,
                include: new System.Linq.Expressions.Expression<Func<Booking, object>>[]
                {
                    b => b.Car, b => b.Customer
                },
                IsTracking: false)
            .FirstOrDefaultAsync();

        if (booking is null)
            return BaseResponse<BookingGetDto>.FailResponse("Booking not found", HttpStatusCode.NotFound);

        var dto = new BookingGetDto
        {
            Id = booking.Id,
            CarId = booking.CarId,
            CarBrand = booking.Car?.Brand,
            CarModel = booking.Car?.Model,
            CustomerId = booking.CustomerId,
            CustomerName = booking.Customer?.FullName,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice,
            DepositAmount = booking.DepositAmount,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };

        return BaseResponse<BookingGetDto>.SuccessResponse(dto, "Booking retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<BookingGetDto>>> GetAllAsync()
    {
        var list = await _bookingRepo
            .GetAll(IsTracking: false)
            .Include(b => b.Car)
            .Include(b => b.Customer)
            .ToListAsync();

        var dtos = list.Select(b => new BookingGetDto
        {
            Id = b.Id,
            CarId = b.CarId,
            CarBrand = b.Car?.Brand,
            CarModel = b.Car?.Model,
            CustomerId = b.CustomerId,
            CustomerName = b.Customer?.FullName,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalPrice = b.TotalPrice,
            DepositAmount = b.DepositAmount,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });

        return BaseResponse<IEnumerable<BookingGetDto>>.SuccessResponse(dtos, "Bookings retrieved successfully", HttpStatusCode.OK);
    }

    

    public async Task<BaseResponse<string>> CreateAsync(BookingCreateDto dto)
    {
       
        if (CurrentUserId is null)
            return BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        
        if (dto.StartDate >= dto.EndDate)
            return BaseResponse<string>.FailResponse("StartDate must be earlier than EndDate.", HttpStatusCode.BadRequest);

        
        if (StartOfDay(dto.StartDate) < DateTime.UtcNow.Date)
            return BaseResponse<string>.FailResponse("You cannot create bookings in the past.", HttpStatusCode.BadRequest);

       
        var car = await _carRepo.GetAll(IsTracking: false).FirstOrDefaultAsync(c => c.Id == dto.CarId);
        if (car is null)
            return BaseResponse<string>.FailResponse("Car not found.", HttpStatusCode.NotFound);

       
        if (car.OwnerId.HasValue && car.OwnerId.Value == CurrentUserId.Value)
            return BaseResponse<string>.FailResponse("You cannot book your own car.", HttpStatusCode.Forbidden);

        
        

        var s = dto.StartDate;
        var e = dto.EndDate; 

        var overlap = await _bookingRepo
            .GetByFiltered(b =>
                b.CarId == dto.CarId &&
                b.Status != BookingStatus.Canceled &&
                b.Status != BookingStatus.Rejected &&
                b.StartDate < e &&           
                b.EndDate > s)               
            .AnyAsync();

        if (overlap)
            return BaseResponse<string>.FailResponse("This car is not available for the selected dates.", HttpStatusCode.Conflict);

       
        var days = CountDays(dto.StartDate, dto.EndDate); 
        var total = car.DailyPrice * days;
        var deposit = Math.Round(total * DepositPercent, 2);

        
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            CarId = dto.CarId,
            CustomerId = CurrentUserId.Value, 
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalPrice = total,
            DepositAmount = deposit,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepo.AddAsync(booking);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(booking.Id.ToString(), "Booking created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string>> UpdateAsync(Guid id, BookingUpdateDto dto)
    {
        if (CurrentUserId is null)
            return BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null)
            return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);

        
        if (booking.CustomerId != CurrentUserId.Value)
            return BaseResponse<string>.FailResponse("You can only update your own booking.", HttpStatusCode.Forbidden);

        if (StartOfDay(booking.StartDate) <= DateTime.UtcNow.Date)
            return BaseResponse<string>.FailResponse("Started or past bookings cannot be modified.", HttpStatusCode.BadRequest);

        
        if (dto.StartDate >= dto.EndDate)
            return BaseResponse<string>.FailResponse("StartDate must be earlier than EndDate.", HttpStatusCode.BadRequest);

        
        var s = dto.StartDate;
        var e = dto.EndDate;

        var overlap = await _bookingRepo
            .GetByFiltered(b =>
                b.CarId == booking.CarId &&
                b.Id != booking.Id &&
                b.Status != BookingStatus.Canceled &&
                b.Status != BookingStatus.Rejected &&
                b.StartDate < e &&
                b.EndDate > s)
            .AnyAsync();
        if (overlap)
            return BaseResponse<string>.FailResponse("This car is not available for the new dates.", HttpStatusCode.Conflict);

        
        var car = await _carRepo.GetByIdAsync(booking.CarId);
        if (car is null)
            return BaseResponse<string>.FailResponse("Car not found", HttpStatusCode.NotFound);

        var days = CountDays(dto.StartDate, dto.EndDate);
        var total = car.DailyPrice * days;
        var deposit = Math.Round(total * DepositPercent, 2);

        booking.StartDate = dto.StartDate;
        booking.EndDate = dto.EndDate;
        booking.TotalPrice = total;
        booking.DepositAmount = deposit;
        booking.Status = dto.Status; 

        _bookingRepo.Update(booking);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(booking.Id.ToString(), "Booking updated successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id)
    {
        if (CurrentUserId is null)
            return BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null)
            return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);

        if (booking.CustomerId != CurrentUserId.Value)
            return BaseResponse<string>.FailResponse("You can only delete your own booking.", HttpStatusCode.Forbidden);

        // soft-cancel (tövsiyə): istəsən hard delete saxla
        booking.Status = BookingStatus.Canceled;
        booking.UpdatedAt = DateTime.UtcNow;

        _bookingRepo.Update(booking);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(booking.Id.ToString(), "Booking canceled", HttpStatusCode.OK);
    }


    public async Task<BaseResponse<IEnumerable<BookingGetDto>>> GetPendingForOwnerAsync()
    {
        if (CurrentUserId is null)
            return BaseResponse<IEnumerable<BookingGetDto>>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        
        var list = await _bookingRepo.GetAll(IsTracking: false)
            .Include(b => b.Car)
            .Include(b => b.Customer)
            .Where(b => b.Status == BookingStatus.Pending
                && b.Car.OwnerId.HasValue
                && b.Car.OwnerId == CurrentUserId.Value)
            .OrderBy(b => b.StartDate)
            .ToListAsync();

        var dtos = list.Select(b => new BookingGetDto
        {
            Id = b.Id,
            CarId = b.CarId,
            CarBrand = b.Car?.Brand,
            CarModel = b.Car?.Model,
            CustomerId = b.CustomerId,
            CustomerName = b.Customer?.FullName,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalPrice = b.TotalPrice,
            DepositAmount = b.DepositAmount,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });

        return BaseResponse<IEnumerable<BookingGetDto>>.SuccessResponse(dtos, "Pending bookings for owner", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> ApproveAsync(Guid id)
    {
        if (CurrentUserId is null)
            return BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Forbidden);

        var b = await _bookingRepo.GetAll(IsTracking: true)
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b is null) return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);
        if (b.Status != BookingStatus.Pending)
            return BaseResponse<string>.FailResponse("Only pending bookings can be approved.", HttpStatusCode.BadRequest);

        
        if (b.Car?.OwnerId != CurrentUserId.Value)
            return BaseResponse<string>.FailResponse("You can only approve bookings for your own cars.", HttpStatusCode.Forbidden);

        
        var newStart = b.StartDate;   
        var newEnd = b.EndDate;     

        
        var overlap = await _bookingRepo.GetByFiltered(x =>
                x.CarId == b.CarId &&
                x.Id != b.Id &&
                x.Status != BookingStatus.Canceled &&
                x.Status != BookingStatus.Rejected &&
                x.StartDate < newEnd &&  
                x.EndDate > newStart)   
            .AnyAsync();
        if (overlap)
            return BaseResponse<string>.FailResponse("Car not available for these dates.", HttpStatusCode.Conflict);

        
        

        b.Status = BookingStatus.Approved;
        b.UpdatedAt = DateTime.UtcNow;
        _bookingRepo.Update(b);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(b.Id.ToString(), "Booking approved", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> RejectAsync(Guid id)
    {
        if (CurrentUserId is null)
            return BaseResponse<string>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        var b = await _bookingRepo.GetAll(IsTracking: true)
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b is null) return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);
        if (b.Status != BookingStatus.Pending)
            return BaseResponse<string>.FailResponse("Only pending bookings can be rejected.", HttpStatusCode.BadRequest);

        if (b.Car?.OwnerId != CurrentUserId.Value)
            return BaseResponse<string>.FailResponse("You can only reject bookings for your own cars.", HttpStatusCode.Forbidden);

        
        

        b.Status = BookingStatus.Rejected;
        b.UpdatedAt = DateTime.UtcNow;
        _bookingRepo.Update(b);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(b.Id.ToString(), "Booking rejected", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> CancelAsync(Guid id, bool byAdmin = false)
    {
        var b = await _bookingRepo.GetByIdAsync(id);
        if (b is null) return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);
        if (b.Status is BookingStatus.Canceled or BookingStatus.Rejected or BookingStatus.Completed)
            return BaseResponse<string>.FailResponse("Booking cannot be canceled in its current state.", HttpStatusCode.BadRequest);

        
        if (!byAdmin)
        {
            var currentUserId = Guid.Parse(_http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (b.CustomerId != currentUserId) return BaseResponse<string>.FailResponse("You can only cancel your own booking.", HttpStatusCode.Forbidden);
            if (b.StartDate.ToDateTime(TimeOnly.MinValue) <= DateTime.UtcNow.Date)
                return BaseResponse<string>.FailResponse("Started or past bookings cannot be canceled.", HttpStatusCode.BadRequest);
        }

        
        var hoursBefore = (b.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).TotalHours;
        var refund = hoursBefore >= 48;

     
        

        b.Status = BookingStatus.Canceled;
        b.UpdatedAt = DateTime.UtcNow;
        _bookingRepo.Update(b);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(b.Id.ToString(), refund ? "Booking canceled, deposit refunded." : "Booking canceled, deposit retained.");
    }

    public async Task<BaseResponse<string>> CompleteAsync(Guid id)
    {
        var b = await _bookingRepo.GetByIdAsync(id);
        if (b is null) return BaseResponse<string>.FailResponse("Booking not found", HttpStatusCode.NotFound);
        if (b.Status != BookingStatus.Approved) return BaseResponse<string>.FailResponse("Only approved bookings can be completed.", HttpStatusCode.BadRequest);

       

        b.Status = BookingStatus.Completed;
        b.UpdatedAt = DateTime.UtcNow;
        _bookingRepo.Update(b);
        await _bookingRepo.SaveChangeAsync();

        return BaseResponse<string>.SuccessResponse(b.Id.ToString(), "Booking completed", HttpStatusCode.OK);
    }
}
