using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Command;
using PlatformEduPro.DTO.Course;
using PlatformEduPro.Models;

namespace PlatformEduPro.Services.Interfaces
{
    public interface ICourseService
    {
        
        Task<Result<CourseDto>> GetCourseByID(int id);
        Task<Result<PaginatedList<CourseDto>>> GetAllCourses(RequestFilter filter);
        Task<Result<List<CourseDto>>> GetValuableCourses();
        Task<Result<List<CourseDto>>> GetLatestCourses();
        Task<Result> AddCourseWithImage(CourseDtoWrite dto);
        Task<Result> UpdateCourseAsync(int id, CourseDtoWrite dto);
        Task<Result> DeleteCourseAsync(int id);
    }
}
