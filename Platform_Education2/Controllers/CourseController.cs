using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PlatformEduPro.DTO.Course;
using PlatformEduPro.DTO;
using PlatformEduPro.Services.Interfaces;
using PlatformEduPro.Contracts.Errors;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.DTO.Command;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   

    public class CourseController : ControllerBase
    {
        ICourseService _course;
        public CourseController(ICourseService courseRepo)
        {
            _course = courseRepo;

        }


        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses([FromQuery] RequestFilter filter)
        {
            var courses = await _course.GetAllCourses(filter);
            return courses.IsSuccess ? Ok(courses.Value) : NotFound();
        }
        [HttpGet("GetLatestCourses")]
        
        public async Task<IActionResult> GetLatestCourses()
        {
            var courses = await _course.GetLatestCourses();
            return courses.IsSuccess ? Ok(courses.Value) : NotFound();
        }
        [HttpGet("GetValuableCourses")]
        public async Task<IActionResult> GetValuableCourses()
        {
            var courses = await _course.GetValuableCourses();
        return courses.IsSuccess ? Ok(courses.Value) : NotFound();
        }


        [HttpGet("Get_CourseBy{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _course.GetCourseByID(id);
            return course.IsSuccess ? Ok(course.Value) : NotFound(course.Error);
        }

        [HttpPost("Add_Course")]
        [HasPermission(Permissions.Course_Create)]
        public async Task<IActionResult> AddCourse([FromForm] CourseDtoWrite dto)
        {
            var result = await _course.AddCourseWithImage(dto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);

        }

        [HttpPut("Update_Course{id}")]
        [HasPermission(Permissions.Course_Update)]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] CourseDtoWrite dto)
        {
            var course = await _course.UpdateCourseAsync(id, dto);
            return course.IsSuccess ? Ok(course.IsSuccess) : BadRequest(course.Error);
        }

        [HttpDelete("Delete_Course{id}")]
        [HasPermission(Permissions.Course_Delete)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _course.DeleteCourseAsync(id);
            return course.IsSuccess ? Ok(course.IsSuccess) : BadRequest(course.Error);
        }

    }
}
