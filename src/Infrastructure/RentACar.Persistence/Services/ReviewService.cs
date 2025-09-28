using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.ReviewDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Persistence.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;

    public ReviewService(IRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<BaseResponse<ReviewGetDto>> CreateAsync(ReviewCreateDto dto)
    {
        var review = new Review
        {
            CarId = dto.CarId,
            CustomerId = dto.CustomerId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangeAsync();

        return BaseResponse<ReviewGetDto>.SuccessResponse(MapToGetDto(review), "Review created successfully", HttpStatusCode.Created);
    }


    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
            return BaseResponse<bool>.FailResponse("Review not found", HttpStatusCode.NotFound);

        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Review deleted successfully");
    }



    public async Task<BaseResponse<IEnumerable<ReviewGetDto>>> GetAllByCarIdAsync(Guid carId)
    {
        var reviews = await _reviewRepository
            .GetByFiltered(r => r.CarId == carId, new[] { (System.Linq.Expressions.Expression<Func<Review, object>>)(r => r.Car), r => r.Customer }, false)
            .Select(r => MapToGetDto(r))
            .ToListAsync();

        return BaseResponse<IEnumerable<ReviewGetDto>>.SuccessResponse(reviews, "Reviews retrieved successfully");
    }

    // 🔹 Mapping helper
    private ReviewGetDto MapToGetDto(Review r)
    {
        return new ReviewGetDto
        {
            Id = r.Id,
            CarId = r.CarId,
            CarBrand = r.Car?.Brand,
            CarModel = r.Car?.Model,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer?.FullName, // əgər User entity-də `FullName` varsa
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
