using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO.Account;
using PlatformEduPro.DTO.Roles;
using PlatformEduPro.Services;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;
        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet("Get_Roles")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetRoles([FromQuery] bool IncludeDisabled,CancellationToken cancellationToken)
        {
            var roles = await roleService.GetRolesAsync(IncludeDisabled,cancellationToken);
            return Ok(roles);
        }


        [HttpGet("{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await roleService.GetAsync(id);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("Add_Role")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> Add([FromBody] RoleRequestDto request)
        {
            var result = await roleService.AddAsync(request);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value) : result.ToProblem();
        }
        [HttpPut("Update_Role{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequestDto request)
        {
            var result = await roleService.UpdateAsync(id, request);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/toggle-status")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await roleService.ToggleStatusAsync(id);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

    }
}
