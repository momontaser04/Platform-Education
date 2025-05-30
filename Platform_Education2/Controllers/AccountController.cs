using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Extensions;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Controllers
{
    [Route("me")]
    [ApiController]
    [Authorize]

    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Info()
        {
            var user = await _userService.getUserProfile(User.GetUserId()!);
            return Ok(user.Value);
        }

        [HttpPut("update_Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatingProfileDto userDto)
        {
            var user = await _userService.UpdateProfile(User.GetUserId()!,userDto);
            if (user.IsSuccess)
            {
                return Ok("Updated Successfuly");
            }
            return BadRequest();
        }
        [HttpPut("Change_Password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto userDto)
        {
            var user = await _userService.UpdatePassword(User.GetUserId()!, userDto);
         return user.IsSuccess ? Ok("Updated Successfuly") : user.ToProblem();
        }

    }
}
