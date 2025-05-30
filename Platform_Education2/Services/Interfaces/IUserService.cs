using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Users;

namespace PlatformEduPro.Services.Interfaces
{
    public interface IUserService
    {
     
        Task<Result<UserProfileDto>> getUserProfile(string id);
        Task<Result> UpdateProfile(string id,UpdatingProfileDto profileDto);
        Task<Result> UpdatePassword(string id, ChangePasswordDto passwordDto);
        Task<IEnumerable<UserResponseDto>> GetAllUsers(CancellationToken cancellationToken);
        Task<Result<UserResponseDto>> GetByID(string ID);
        Task<Result<CreateUserDto>> CreateUser(CreateUserDto userDto, CancellationToken cancellationToken);
        Task<Result> UpdateUser(string Id, UpdateUserDto userDto, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(string id);

        Task<Result> Unlock(string id);





    }
}
