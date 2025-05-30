using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO.Users;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService= userService;
        }

        [HttpGet("GetAllUsers")]
        [HasPermission(Permissions.GetUsers)]

        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllUsers(cancellationToken);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }
        [HttpGet("GetById/{id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var result = await _userService.GetByID(id);
          return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("CreateUser")]
        [HasPermission(Permissions.AddUsers)]

        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto, CancellationToken cancellationToken)
        {
            var result = await _userService.CreateUser(userDto, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }



        [HttpPut("UpdateUser/{id}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto userDto, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUser(id, userDto, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPut("ToggleStatus/{id}")]
        [HasPermission(Permissions.ToggleStatus)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await _userService.ToggleStatusAsync(id);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }


        [HttpPut("UnlockUser/{id}")]
        [HasPermission(Permissions.UnlockUsers)]
        public async Task<IActionResult> UnlockUser([FromRoute] string id)
        {
            var result = await _userService.Unlock(id);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }



    }
}
