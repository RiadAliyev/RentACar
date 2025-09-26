using RentACar.Application.DTOs.RoleDtos;
using RentACar.Application.Shared;

namespace RentACar.Application.Abstracts.Services;

public interface IRoleService
{
    Task<BaseResponse<string?>> CreateRole(RoleCreateDto dto);
    Task<BaseResponse<string?>> UpdateRole(RoleUpdateDto dto);
    Task<BaseResponse<string?>> DeleteRole(string roleName);
    BaseResponse<List<RoleListDto>> GetAllRoles();
}
