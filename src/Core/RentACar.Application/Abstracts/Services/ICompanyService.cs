using RentACar.Application.DTOs.CompanyDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface ICompanyService
{
    Task<BaseResponse<CompanyGetDto>> CreateAsync(CompanyCreateDto dto, Guid ownerId);
    Task<BaseResponse<CompanyGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<CompanyGetDto>>> GetAllAsync();
    Task<BaseResponse<CompanyGetDto>> UpdateAsync(Guid id, CompanyUpdateDto dto);
    Task<BaseResponse<bool>> DeleteAsync(Guid id);
    Task<BaseResponse<IEnumerable<CompanyGetDto>>> GetByFilterAsync(CompanyFilterDto filter);
}
