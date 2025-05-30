using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO.Course;
using PlatformEduPro.DTO.Question;
using PlatformEduPro.DTO.Section;
using PlatformEduPro.Services;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting(RateLimiters.IpLimiter)]
    public class SectionController : ControllerBase
    {
        ISctionService _sctionRepo;
        public SectionController(ISctionService sctionRepo)
        {
            _sctionRepo = sctionRepo;
        }




        [HttpGet]
        [HasPermission(Permissions.Section_GetAll)]

        public async Task<IActionResult> GetSections()
        {
            var result = await _sctionRepo.GetAllSections();
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.Section_GetById)]
        public async Task<IActionResult> GetSectionById(int id)
        {
            var result = await _sctionRepo.GetSectionById(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
        [HttpPost("AddSection")]
        [HasPermission(Permissions.Section_Create)]
   
        public async Task<IActionResult> AddSection([FromBody] WriteSectionDto dto)
        {
            var result = await _sctionRepo.AddSection(dto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }



       


        [HttpPut("{id}")]
        [HasPermission(Permissions.Section_Update)]
       
        public async Task<IActionResult> UpdateSection(int id, [FromBody] WriteSectionDto dto)
        {
            var result = await _sctionRepo.UpdateSection(id, dto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.Section_Delete)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _sctionRepo.DeleteSection(id);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }



    }
}
