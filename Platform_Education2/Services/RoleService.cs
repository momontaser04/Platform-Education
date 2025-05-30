using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Errors;
using PlatformEduPro.DTO.Roles;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using System.Threading;

namespace PlatformEduPro.Services
{
    public class RoleService(RoleManager<AppRole> roleManager,EduPlatformDbContext context) : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager = roleManager;
        private readonly EduPlatformDbContext _context = context;

        public async Task<IEnumerable<RoleDto>> GetRolesAsync(bool? includeDisabled=false, CancellationToken cancellationToken = default)
        {
            var roles = await _roleManager.Roles.
                Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
                .Select(x => new RoleDto
                {
                    Id = x.Id.ToString(), 
                    Name = x.Name,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync(cancellationToken);
             
            return roles;
        }
        public async Task<Result<RoleDetailedDto>> GetAsync(string id)
        {
            var role=await _roleManager.FindByIdAsync(id);

            if(role == null)
            {
                return Result.Failure<RoleDetailedDto>(RoleError.RoleNotFound);
            }

            var permissions = await _roleManager.GetClaimsAsync(role);

            var response = new RoleDetailedDto(
         role.Id,
         role.Name!,
         role.IsDeleted,
         permissions.Select(x => x.Value));


            return Result.Success(response);

        }


        public async Task<Result<RoleDetailedDto>> AddAsync(RoleRequestDto request)
        {
            var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);

            if (roleIsExists)
                return Result.Failure<RoleDetailedDto>(RoleError.DuplicatedRole);

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailedDto>(RoleError.InvalidPermissions);

            var role = new AppRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                var permissions = request.Permissions
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });

                await _context.AddRangeAsync(permissions);
                await _context.SaveChangesAsync();

                var response = new RoleDetailedDto(role.Id, role.Name, role.IsDeleted, request.Permissions);

                return Result.Success(response);
            }

            var error = result.Errors.First();

            return Result.Failure<RoleDetailedDto>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

 

        public async Task<Result> UpdateAsync(string id, RoleRequestDto request)
        {
            var roleIsExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

            if (roleIsExists)
                return Result.Failure<RoleDetailedDto>(RoleError.DuplicatedRole);

            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailedDto>(RoleError.RoleNotFound);

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailedDto>(RoleError.InvalidPermissions);

            role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var currentPermissions = await _context.RoleClaims
                    .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                    .Select(x => x.ClaimValue)
                    .ToListAsync();

                var newPermissions = request.Permissions.Except(currentPermissions)
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id
                    });

                var removedPermissions = currentPermissions.Except(request.Permissions);

                await _context.RoleClaims
                    .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();


                await _context.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure<RoleDetailedDto>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }



        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailedDto>(RoleError.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;

            await _roleManager.UpdateAsync(role);

            return Result.Success();
        }
    }

}
