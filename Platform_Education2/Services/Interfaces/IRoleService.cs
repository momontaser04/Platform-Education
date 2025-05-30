using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Roles;

namespace PlatformEduPro.Services.Interfaces
{
    public interface IRoleService
    {
       Task<IEnumerable<RoleDto>> GetRolesAsync(bool? IncludeDisabled,CancellationToken cancellationToken=default);
        Task<Result<RoleDetailedDto>> GetAsync(string id);
        Task<Result<RoleDetailedDto>> AddAsync(RoleRequestDto request);
        Task<Result> UpdateAsync(string id, RoleRequestDto request);
        Task<Result> ToggleStatusAsync(string id);
      
    }
}
