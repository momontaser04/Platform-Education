using PlatformEduPro.Contracts.Errors;

namespace PlatformEduPro.Contracts.ErrorHandling
{
    public class CourseError
    {
        public static readonly Error CourseNOtFound =
           new("Course.CourseIDNOtFound", "NoCourse Was found With The Given Id", StatusCodes.Status404NotFound);

        public static readonly Error CourseAdding =
          new("Course.CourseAddingError", "InvalidCourseFeatures", StatusCodes.Status401Unauthorized);
    }
}
