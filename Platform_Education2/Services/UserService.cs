using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Errors;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = PlatformEduPro.Contracts.Errors.Error;

namespace PlatformEduPro.Services
{
    public class UserService: IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EduPlatformDbContext _context;
        private readonly IRoleService _roleService;
        public UserService(UserManager<AppUser> userManager, EduPlatformDbContext context,IRoleService roleService)
        {
            _userManager = userManager;
            _context = context;
            _roleService = roleService;

        }


        public async Task<Result<UserProfileDto>> getUserProfile(string id)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == id)
                .Select(x => new UserProfileDto
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber
                })
                .SingleOrDefaultAsync();

        
            return Result.Success(user);
        }

  

        public async Task<Result> UpdateProfile(string id, UpdatingProfileDto profileDto)
        {
            var user = await _userManager.Users
                 .Where(a => a.Id == id).
                SingleOrDefaultAsync();
            user.FirstName = profileDto.FirstName;
            user.LastName = profileDto.LastName;
            user.PhoneNumber = profileDto.PhoneNumber;
            var updateResult = await _userManager.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> UpdatePassword(string id, ChangePasswordDto passwordDto)
        {
      
            var user = await _userManager.Users
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
            var result = await _userManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);
            if(result.Succeeded)
            {
                return Result.Success();

            }
            var errors = result.Errors.First();
            return Result.Failure(new Error(errors.Code,errors.Description,StatusCode:StatusCodes.Status400BadRequest));
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsers(CancellationToken cancellationToken) =>
            await (from u in _context.Users
                   join ur in _context.UserRoles on u.Id equals ur.UserId
                   join r in _context.Roles on ur.RoleId equals r.Id into roles
                   where !roles.Any(z => z.Name == DefaultRoles.Member)
                   select new 
                   {
                       ID = u.Id,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       Email = u.Email!,
                       PhoneNumber = u.PhoneNumber!,
                       IsDisabled = u.IsDisabled,
                       Roles = roles.Select(r => r.Name).ToList()!
                   }).GroupBy(u => new{ u.ID,u.Email,u.FirstName,u.LastName,u.PhoneNumber,u.IsDisabled})
            .Select(u=>new UserResponseDto
            {
                ID = u.Key.ID,
                FirstName = u.Key.FirstName,
                LastName = u.Key.LastName,
                Email = u.Key.Email!,
                PhoneNumber = u.Key.PhoneNumber,
                IsDisabled = u.Key.IsDisabled,
                Roles = u.SelectMany(x => x.Roles).Distinct().ToList()
            })
            
            .ToListAsync(cancellationToken);
       


        public async Task<Result<UserResponseDto>> GetByID(string ID)
        {
            var user =await _userManager.FindByIdAsync(ID);
            if(user == null)
            {
                return Result.Failure<UserResponseDto>(UseError.UserNotFound);
            }
            var userroles = await _userManager.GetRolesAsync(user);

            var result=new UserResponseDto
            {
                ID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                IsDisabled = user.IsDisabled,
                Roles = userroles.ToList()
            };

            return Result.Success(result);

        }


        public async Task<Result<CreateUserDto>> CreateUser(CreateUserDto userDto, CancellationToken cancellationToken)
        {
            var EmailISExist = await _userManager.Users.AnyAsync(x => x.Email == userDto.Email);
            if (EmailISExist)
            {
                return Result.Failure<CreateUserDto>(UseError.DuplicatedEmail);
            }
            var allowedRoles = await _roleService.GetRolesAsync(false, cancellationToken: cancellationToken);

            if (userDto.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            {
                return Result.Failure<CreateUserDto>(UseError.InvalidRole);
            }
            var user = new AppUser
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };
            user.EmailConfirmed = true;

          

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userDto.Roles);

                var response = new CreateUserDto
                {
               
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = userDto.Roles
                };

                return Result.Success(response);
            }

            var errors = result.Errors.FirstOrDefault();
            return Result.Failure<CreateUserDto>(new Error( errors.Code, errors.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result>  UpdateUser(string Id,UpdateUserDto userDto, CancellationToken cancellationToken)
        {
            var EmailISExist = await _userManager.Users.AnyAsync(x => x.Email == userDto.Email&&x.Id!=Id);
            if (EmailISExist)
            {
                return Result.Failure<CreateUserDto>(UseError.UserNotFound);
            }
            var allowedRoles = await _roleService.GetRolesAsync(false, cancellationToken: cancellationToken);

            if (userDto.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            {
                return Result.Failure(UseError.InvalidRole);
            }
            if(await _userManager.FindByIdAsync(Id) is not { } user)
            {
                return Result.Failure(UseError.UserNotFound);
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.UserName = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Email = userDto.Email;


            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.FirstOrDefault();
                return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
            }
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            await _userManager.AddToRolesAsync(user, userDto.Roles);

            return Result.Success();
        }
        public async Task<Result> ToggleStatusAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.Failure(UseError.UserNotFound);
            }

            user.IsDisabled = !user.IsDisabled;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            var errors = result.Errors.FirstOrDefault();
            return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> Unlock(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UseError.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }





    }
}
