using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Authorization;
using PlatformEduPro.DTO.Account;
using PlatformEduPro.DTO.Users;

namespace PlatformEduPro.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password,CancellationToken cancellationToken=default );
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default );
        Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default );
        Task<Result> RegisterAsync(NewUserDto newUser, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(EmailConfirmDto request);
        Task<Result> ResendConfirmationAsync(ResendVerificationDto request);
        Task<Result> SendResetPasswordAsync(ForgetPasswordDto Request);
        Task<Result> ResetPasswordAsync(ResetPasswordDto request);
    }
}
