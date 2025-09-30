using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.ReviewDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RentACar.Persistence.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Car> _carRepository;
    private readonly IHttpContextAccessor _http;

    public ReviewService(
        IRepository<Review> reviewRepository,
        IRepository<Car> carRepository,
        IHttpContextAccessor http)
    {
        _reviewRepository = reviewRepository;
        _carRepository = carRepository;
        _http = http;
    }

    private Guid? CurrentUserId
    {
        get
        {
            var id = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? _http.HttpContext?.User.FindFirst("sub")?.Value;

            return Guid.TryParse(id, out var g) ? g : null;
        }
    }

    

    public async Task<BaseResponse<ReviewGetDto>> CreateAsync(ReviewCreateDto dto)
    {
        
        if (CurrentUserId is null)
            return BaseResponse<ReviewGetDto>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

       
        if (dto.Rating < 1 || dto.Rating > 5)
            return BaseResponse<ReviewGetDto>.FailResponse("Rating must be between 1 and 5.", HttpStatusCode.BadRequest);

        
        var carExists = await _carRepository.GetByFiltered(c => c.Id == dto.CarId).AnyAsync();
        if (!carExists)
            return BaseResponse<ReviewGetDto>.FailResponse("Car not found.", HttpStatusCode.NotFound);

      
        var already = await _reviewRepository
            .GetByFiltered(r => r.CarId == dto.CarId && r.CustomerId == CurrentUserId.Value)
            .AnyAsync();
        if (already)
            return BaseResponse<ReviewGetDto>.FailResponse("You have already reviewed this car.", HttpStatusCode.Conflict);

        
        var review = new Review
        {
            CarId = dto.CarId,
            CustomerId = CurrentUserId.Value,     
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangeAsync();

        
        var createdDto = await _reviewRepository
            .GetByFiltered(r => r.Id == review.Id, IsTracking: false)
            .Select(r => new ReviewGetDto
            {
                Id = r.Id,
                CarId = r.CarId,
                CarBrand = r.Car != null ? r.Car.Brand : null,
                CarModel = r.Car != null ? r.Car.Model : null,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer != null ? r.Customer.FullName : null,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .FirstAsync();

        return BaseResponse<ReviewGetDto>.SuccessResponse(createdDto, "Review created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        if (CurrentUserId is null)
            return BaseResponse<bool>.FailResponse("Unauthorized", HttpStatusCode.Unauthorized);

        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
            return BaseResponse<bool>.FailResponse("Review not found", HttpStatusCode.NotFound);

        
        if (review.CustomerId != CurrentUserId.Value)
            return BaseResponse<bool>.FailResponse("You can only delete your own review.", HttpStatusCode.Forbidden);

        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Review deleted successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<ReviewGetDto>>> GetAllByCarIdAsync(Guid carId)
    {
        var reviews = await _reviewRepository
            .GetByFiltered(r => r.CarId == carId,  
                include: null, IsTracking: false)
            .Select(r => new ReviewGetDto
            {
                Id = r.Id,
                CarId = r.CarId,
                CarBrand = r.Car != null ? r.Car.Brand : null,
                CarModel = r.Car != null ? r.Car.Model : null,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer != null ? r.Customer.FullName : null,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return BaseResponse<IEnumerable<ReviewGetDto>>.SuccessResponse(reviews, "Reviews retrieved successfully", HttpStatusCode.OK);
    }

    
    
}
