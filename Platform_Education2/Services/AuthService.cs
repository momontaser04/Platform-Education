using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Authentication;
using PlatformEduPro.Contracts.Authorization;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Errors;
using PlatformEduPro.DTO.Account;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Helper;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PlatformEduPro.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<AppUser> _userManager;
        private IJwtProvider _jwtProvider;
        private SignInManager<AppUser> _signInManager;  
        private ILogger<AuthResponse> _logger;
        private IEmailSender _EmailSender;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly EduPlatformDbContext _context;

        private readonly int _refreshtokenexpiryday = 14;

        public AuthService(UserManager<AppUser> userManager,IJwtProvider jwtProvider, SignInManager<AppUser> signInManager,ILogger<AuthResponse>logger,IEmailSender emailSender,IHttpContextAccessor httpContextAccessor, EduPlatformDbContext context)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _signInManager = signInManager;
            _logger = logger;
            _EmailSender= emailSender;
            _httpContextAccessor = httpContextAccessor;
            _context = context;

        }
        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {
            //check user 
            var user = await _userManager.FindByEmailAsync(Email);

            if(user == null)
            {
                return Result.Failure<AuthResponse>(UseError.InvalidCredentials);
            }
            if (user.IsDisabled)
            {
                return Result.Failure<AuthResponse>(UseError.DisabledUser);
            }

            if(user.LockoutEnd>DateTime.UtcNow)
            {
                return Result.Failure<AuthResponse>(UseError.LockedUser);
            }
            //check password 

     var result=await _signInManager.PasswordSignInAsync(user, Password, false,true);
             
            if (result.Succeeded)
            {

                var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
                //generate jwt token
                var (token, ExpiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);


                //generate refresh token

                var refreshtoken = GenerateRefreshToken();

                var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_refreshtokenexpiryday);

                //save it in database

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshtoken,
                    ExpiresOn = RefreshTokenExpiryDate
                });
                await _userManager.UpdateAsync(user);

                //return new AuthResponse()
                var newresult = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, ExpiresIn, refreshtoken, RefreshTokenExpiryDate);

                return Result.Success(newresult);

            }

            var errors= result.IsNotAllowed
            ? UseError.EmailNotConfirmed
            : result.IsLockedOut
            ? UseError.LockedUser
            : UseError.InvalidCredentials;

            return Result.Failure<AuthResponse>(errors);




        }






        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);

            if (userId == null)
            {
                return Result.Failure<AuthResponse>(UseError.InvalidJwtToken);
            }
            var user =await _userManager.FindByIdAsync(userId); 
            if (user == null)
            {
                return Result.Failure<AuthResponse>(UseError.EmailNotConfirmed);

            }
            if (user.IsDisabled)
            {
                return Result.Failure<AuthResponse>(UseError.DisabledUser);
            }

            var refreshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);

           



            if(refreshtoken == null)
            {
                return Result.Failure<AuthResponse>(UseError.InvalidRefreshToken); 
            }
            refreshtoken.RevokedOn = DateTime.UtcNow;

            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            //generate jwt token
            var (newtoken, ExpiresIn) = _jwtProvider.GenerateToken(user,userRoles,userPermissions);


            //generate refresh token

            var newrefreshtoken = GenerateRefreshToken();

            var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_refreshtokenexpiryday);

            //save it in database

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newrefreshtoken,
                ExpiresOn = RefreshTokenExpiryDate
            });
            await _userManager.UpdateAsync(user);
            var result= new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newtoken, ExpiresIn, newrefreshtoken, RefreshTokenExpiryDate);

            return Result.Success(result);


        }




      

        public async Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);

            if (userId == null)
            {
                return Result.Failure<AuthResponse>(UseError.InvalidJwtToken);
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure<AuthResponse>(UseError.EmailNotConfirmed);

            }

            var refreshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);





            if (refreshtoken == null)
            {
                return Result.Failure<AuthResponse>(UseError.InvalidRefreshToken);
            }
            refreshtoken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Result.Success();


        }
        //if you want to make the user autologin 
        // 2) return get token into usermanager 
        // 3) Cancel options .signin.requiredconfirmationEmail

        //        public async Task<Result<AuthResponse>> RegisterAsync(DtoNewUser newUser, CancellationToken cancellationToken = default)
        //        {
        //            var emailExists = await _userManager.Users.AnyAsync(p => p.Email == newUser.Email, cancellationToken);

        //            if (emailExists)
        //            {
        //                return Result.Failure<AuthResponse>(UseError.DuplicatedEmail);
        //            }

        //            AppUser user = new AppUser()
        //            {
        //                UserName = newUser.Email,
        //                FirstName = newUser.firstName,
        //                LastName = newUser.lastName,
        //                Email = newUser.Email,
        //                PhoneNumber = newUser.PhoneNumber,
        //            };

        //            var result = await _userManager.CreateAsync(user, newUser.password);

        //            if (result.Succeeded)
        //            {
        //                var (token, ExpiresIn) = _jwtProvider.GenerateToken(user);


        //                //generate refresh token

        //                var refreshtoken = GenerateRefreshToken();

        //                var RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(_refreshtokenexpiryday);

        //                //save it in database

        //                user.RefreshTokens.Add(new RefreshToken
        //                {
        //                    Token = refreshtoken,
        //                    ExpiresOn = RefreshTokenExpiryDate
        //                });
        //                await _userManager.UpdateAsync(user);
        //                var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, ExpiresIn, refreshtoken, RefreshTokenExpiryDate);
        //                return Result.Success(response);

        //            }

        //            var error = result.Errors.First();

        //            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest)); 

        //}

        public async Task<Result> RegisterAsync(NewUserDto newUser, CancellationToken cancellationToken = default)
        {
            var emailExists = await _userManager.Users.AnyAsync(p => p.Email == newUser.Email, cancellationToken);

            if (emailExists)
            {
                return Result.Failure<AuthResponse>(UseError.DuplicatedEmail);
            }

            AppUser user = new AppUser()
            {
                UserName = newUser.Email,
                FirstName = newUser.firstName,
                LastName = newUser.lastName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, newUser.password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                _logger.LogInformation($"Confirmation Code: {code}");

               
                // send confirmation Email
                await SendConfirmationEmail(user, code);
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
                return Result.Success();

            }

         


         
            

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }

        public async Task<Result> ConfirmEmailAsync(EmailConfirmDto request)
        {
            var user = await _userManager.FindByIdAsync(request.ID);

            if (user==null)
            {
                return Result.Failure(UseError.InvalidCode);
            }
            //هنشوف لو الايميل حصله كونفيرم يبقي في خطأ
       if(user.EmailConfirmed)
            {

                return Result.Failure(UseError.DuplicatedConfirmation);
            }

            var code=request.Code;
            try
            {
                code=Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Result.Failure(UseError.InvalidCode);
            }
            var result=await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ResendConfirmationAsync(ResendVerificationDto request)
        {
            var user=await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Success();
            }
            if (user.EmailConfirmed)
            {
                return Result.Failure(UseError.DuplicatedConfirmation);
            }
            //Generate Code
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation($"Confirmation Code: {code}");

            //the code sent to controller
            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> SendResetPasswordAsync(ForgetPasswordDto Request)
        {
            var user=await _userManager.FindByEmailAsync(Request.Email);
            if (user == null)
            {
                //Misleading the user if he tries to hack
                return Result.Success();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation($"Reset Code: {code}");

            //the code sent to controller
            await SendResetPasswordEmail(user, code);

            return Result.Success();


        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(UseError.InvalidCode);
            }
            //هنشوف لو الايميل حصله كونفيرم يبقي في خطأ
            if (user.EmailConfirmed)
            {

                return Result.Failure(UseError.DuplicatedConfirmation);
            }

            var code = request.code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Result.Failure(UseError.InvalidCode);
            }

            var result = await _userManager.ResetPasswordAsync(user, code, request.Password);

            if (result.Succeeded)
            {
                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }



        public async Task SendResetPasswordEmail(AppUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/ForgetPassword?Email={user.Email}&code={code}" }
                }
            );
            //Send Email Using Hangfire
            BackgroundJob.Enqueue(() => _EmailSender.SendEmailAsync(user.Email!, "✅ Platform Education: Change Password", emailBody));
            await Task.CompletedTask;

        }

        public async Task SendConfirmationEmail(AppUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/EmailConfirmation?userId={user.Id}&code={code}" }
                }
            );
            //Send Email Using Hangfire
            BackgroundJob.Enqueue(() => _EmailSender.SendEmailAsync(user.Email!, "✅ Platform Education: Email Confirmation", emailBody));
            await Task.CompletedTask;
         
        }



        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(AppUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var userPermissions = await (from r in _context.Roles
                                         join p in _context.RoleClaims
                                         on r.Id equals p.RoleId
                                         where userRoles.Contains(r.Name!)
                                         select p.ClaimValue!)
                                         .Distinct()
                                         .ToListAsync(cancellationToken);

            return (userRoles, userPermissions);
        }



        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
