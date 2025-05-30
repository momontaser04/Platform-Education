using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Account;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Extensions;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using System.Data;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private UserManager<AppUser> UserManager;
 
       
        public AuthController(IAuthService authService,UserManager<AppUser> userManager,IUserService user)
        {
            _authService = authService;
          
            UserManager=userManager;
       
            
        }

        [HttpPost("Register")]

        public async Task<IActionResult> RegisterNewUser([FromBody] NewUserDto newUser,CancellationToken cancellationToken)
        {
            var authresult = await _authService.RegisterAsync(newUser, cancellationToken);

            return authresult.IsSuccess ? Ok() : authresult.ToProblem();



        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request,CancellationToken cancellationToken)
        {
            var authresult=await _authService.GetTokenAsync(request.Email,request.Password,cancellationToken);

            return authresult.IsSuccess ? Ok(authresult.Value) : authresult.ToProblem();
        }


        [HttpPost("Refresh_Token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var authresult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authresult.IsSuccess ? Ok(authresult.Value) : authresult.ToProblem();
        }






        [HttpPost("Revoke_Refresh_Token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var authresult = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

            return authresult.IsSuccess ? Ok(authresult) : authresult.ToProblem();
        }

        [HttpPost("Confirm_Email")]
        public async Task<IActionResult> ConfirmationEmail([FromBody] EmailConfirmDto request)
        {
            var authresult = await _authService.ConfirmEmailAsync(request);

            return authresult.IsSuccess ? Ok(authresult) : authresult.ToProblem();
        }


        [HttpPost("Resend_Confirm_Email")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto request)
        {
            var authresult = await _authService.ResendConfirmationAsync(request);

            return authresult.IsSuccess ? Ok(authresult) : authresult.ToProblem();
        }

        [HttpPost("Forget_Password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto request)
        {
            var authresult = await _authService.SendResetPasswordAsync(request);

            return authresult.IsSuccess ? Ok(authresult) : authresult.ToProblem();
        }


        [HttpPost("Reset_Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var authresult = await _authService.ResetPasswordAsync(request);

            return authresult.IsSuccess ? Ok(authresult) : authresult.ToProblem();
        }
      

    }
}
