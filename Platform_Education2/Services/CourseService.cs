using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.DTO.Course;
using PlatformEduPro.DTO;
using PlatformEduPro.Contracts.ErrorHandling;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Command;
using System.Linq.Dynamic.Core;

namespace EDU_Platform.Services
{
    public class CourseService : ICourseService
    {
        EduPlatformDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CourseService(EduPlatformDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        //تطبيق pagination and sorting للتجربه والتعلم
        public async Task<Result<PaginatedList<CourseDto>>> GetAllCourses(RequestFilter filter)
        {
            var query = _context.TbCourses
                .AsNoTracking()
                .Include(c => c.images) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                query = query.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
            }

            var projectedQuery = query.Select(c => new CourseDto
            {
                Id = c.Id,
                CourseName = c.CourseName,
                Description = c.Description,
                SalesPrice = c.SalesPrice,
                Imagefile = c.images != null ? c.images.ImagePath : null
            });

            var paginatedCourses = await PaginatedList<CourseDto>.CreateAsync(projectedQuery, filter.PageNumber, filter.PageSize);

            return Result.Success(paginatedCourses);
        }


        public async Task<Result<List<CourseDto>>> GetLatestCourses()
        {
            var courses = _context.TbCourses
            .OrderByDescending(c => c.CreatedDate)
            .Take(3)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                CourseName = c.CourseName,
                SalesPrice = c.SalesPrice,
                Description = c.Description,
                Imagefile = c.images != null ? c.images.ImagePath : null 
            })
            .ToList();

            return Result.Success(courses);


        }
        public async Task<Result<List<CourseDto>>> GetValuableCourses()
        {


            var courses = _context.TbCourses
           .OrderByDescending(c => c.SalesPrice)
           .Take(3)
           .Select(c => new CourseDto
           {
               Id = c.Id,
               CourseName = c.CourseName,
               SalesPrice = c.SalesPrice,
               Description = c.Description,
               Imagefile = c.images != null ? c.images.ImagePath : null
           })
           .ToList();

            return Result.Success(courses);
        }

        public async Task<Result<CourseDto>> GetCourseByID(int id)
        {
            var course = await _context.TbCourses
                .Include(c => c.images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (course == null)
            {
                return Result.Failure<CourseDto>(CourseError.CourseNOtFound);
            }

            var courseDto = new CourseDto
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Description = course.Description,
                SalesPrice = course.SalesPrice,
                Imagefile = course.images?.ImagePath // Safe navigation in case images is null
            };

            return Result.Success(courseDto);
        }





        public async Task<Result> AddCourseWithImage(CourseDtoWrite dto)
        {
            try
            {
              
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.Imagefile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Imagefile.CopyToAsync(fileStream);
                }

              
                var course = new TbCourses
                {
                    CourseName = dto.CourseName,
                    Description = dto.Description,
                    SalesPrice = dto.SalesPrice,
                    images = new TbImages
                    {
                        ImagePath = "/images/" + uniqueFileName
                    }
                };

                _context.TbCourses.Add(course);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Result.Failure(CourseError.CourseAdding);
            }
        }



        public async Task<Result> UpdateCourseAsync(int id, CourseDtoWrite dto)
        {
            var course = await _context.TbCourses
                .Include(c => c.images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return Result.Failure(CourseError.CourseNOtFound);

            // تحديث بيانات الدورة
            course.CourseName = dto.CourseName;
            course.Description = dto.Description;
            course.SalesPrice = dto.SalesPrice;

            if (dto.Imagefile != null)
            {
                // حذف الصورة القديمة إذا كانت موجودة
                if (course.images != null && !string.IsNullOrEmpty(course.images.ImagePath))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, course.images.ImagePath.TrimStart('/'));
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }
                else
                {
                    // إذا لم تكن هناك صورة، نُنشئ كائناً جديدًا
                    course.images = new TbImages { CourseId = course.Id };
                    _context.TbImages.Add(course.images);
                }

                // حفظ الصورة الجديدة
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagefile.FileName)}";
                string filePath = Path.Combine(_env.WebRootPath, "Images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Imagefile.CopyToAsync(stream);
                }

                // تحديث مسار الصورة
                course.images.ImagePath = $"/Images/{fileName}";
            }

            _context.TbCourses.Update(course);
            await _context.SaveChangesAsync();
            return Result.Success();
        }









        public async Task<Result> DeleteCourseAsync(int id)
        {
            var course = await _context.TbCourses.Include(c => c.images).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return Result.Failure(CourseError.CourseNOtFound);

            if (course.images != null)
            {
                string oldPath = Path.Combine(_env.WebRootPath, course.images.ImagePath.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);

                _context.TbImages.Remove(course.images);
            }

            _context.TbCourses.Remove(course);
            await _context.SaveChangesAsync();
            return Result.Success();
        }







    }
}
