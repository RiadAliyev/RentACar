using RentACar.Application.Abstracts.Repositories;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CompanyDtos;
using RentACar.Application.Shared;
using RentACar.Domain.Entities;
using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Persistence.Services;

public class CompanyService : ICompanyService
{
    private readonly IRepository<Company> _companyRepo;

    public CompanyService(IRepository<Company> companyRepo)
    {
        _companyRepo = companyRepo;
    }

    public async Task<BaseResponse<CompanyGetDto>> CreateAsync(CompanyCreateDto dto, Guid ownerId)
    {
        var alreadyHas = await _companyRepo.GetAll(IsTracking: false)
            .AnyAsync(c => c.OwnerId == ownerId);
        if (alreadyHas)
            return BaseResponse<CompanyGetDto>.FailResponse("You already have a company.", HttpStatusCode.BadRequest);


        var entity = new Company
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = dto.Name,
            RegistrationNumber = dto.RegistrationNumber,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepo.AddAsync(entity);
        await _companyRepo.SaveChangeAsync();

        var result = MapToGetDto(entity);
        return BaseResponse<CompanyGetDto>.SuccessResponse(result, "Company created successfully", HttpStatusCode.Created);
    }

    public async Task<BaseResponse<CompanyGetDto>> GetByIdAsync(Guid id)
    {
        var entity = await _companyRepo.GetByFiltered(
            x => x.Id == id,
            include: new Expression<Func<Company, object>>[] { x => x.Owner, x => x.Cars },
            IsTracking: false
        ).FirstOrDefaultAsync();

        if (entity is null)
            return BaseResponse<CompanyGetDto>.FailResponse("Company not found", HttpStatusCode.NotFound);

        var result = MapToGetDto(entity);
        return BaseResponse<CompanyGetDto>.SuccessResponse(result);
    }

    public async Task<BaseResponse<IEnumerable<CompanyGetDto>>> GetAllAsync()
    {
        var entities = await _companyRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Cars)
            .ToListAsync();

        var result = entities.Select(MapToGetDto);
        return BaseResponse<IEnumerable<CompanyGetDto>>.SuccessResponse(result, "Companies retrieved successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<CompanyGetDto>> UpdateAsync(Guid id, CompanyUpdateDto dto)
    {
        var entity = await _companyRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<CompanyGetDto>.FailResponse("Company not found", HttpStatusCode.NotFound);

        entity.Name = dto.Name;
        entity.RegistrationNumber = dto.RegistrationNumber;
        entity.Address = dto.Address;
        entity.UpdatedAt = DateTime.UtcNow;

        _companyRepo.Update(entity);
        await _companyRepo.SaveChangeAsync();

        var result = MapToGetDto(entity);
        return BaseResponse<CompanyGetDto>.SuccessResponse(result, "Company updated successfully", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
    {
        var entity = await _companyRepo.GetByIdAsync(id);
        if (entity is null)
            return BaseResponse<bool>.FailResponse("Company not found", HttpStatusCode.NotFound);

        _companyRepo.Delete(entity);
        await _companyRepo.SaveChangeAsync();

        return BaseResponse<bool>.SuccessResponse(true, "Company deleted successfully", HttpStatusCode.OK);
    }

    
    private CompanyGetDto MapToGetDto(Company e)
    {
        return new CompanyGetDto
        {
            Id = e.Id,
            Name = e.Name,
            RegistrationNumber = e.RegistrationNumber,
            Address = e.Address,
            OwnerId = e.OwnerId,
            OwnerName = e.Owner?.FullName ?? string.Empty, 
            CreatedAt = e.CreatedAt,
            CarNames = e.Cars?.Select(c => $"{c.Brand} {c.Model}").ToList() ?? new()
        };
    }

    public async Task<BaseResponse<IEnumerable<CompanyGetDto>>> GetByFilterAsync(CompanyFilterDto filter)
    {
        var query = _companyRepo.GetAll(IsTracking: false)
            .Include(x => x.Owner)
            .Include(x => x.Cars)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(c => c.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.RegistrationNumber))
            query = query.Where(c => c.RegistrationNumber.Contains(filter.RegistrationNumber));

        if (!string.IsNullOrWhiteSpace(filter.Address))
            query = query.Where(c => c.Address.Contains(filter.Address));

        if (filter.OwnerId.HasValue)
            query = query.Where(c => c.OwnerId == filter.OwnerId.Value);

        var entities = await query.ToListAsync();

        var result = entities.Select(MapToGetDto);
        return BaseResponse<IEnumerable<CompanyGetDto>>.SuccessResponse(result, "Filtered companies retrieved successfully");
    }

}