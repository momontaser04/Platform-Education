using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم الكورس مطلوب")]
        [MinLength(3, ErrorMessage = "يجب أن يكون اسم الكورس على الأقل 3 أحرف")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "سعر الكورس مطلوب")]
        public double SalesPrice { get; set; }
        public string Imagefile { get; set; }

        [Required(ErrorMessage = "وصف الكورس مطلوب")]
        [MinLength(10, ErrorMessage = "يجب أن يكون الوصف على الأقل 10 أحرف")]
        public string Description { get; set; }

    }

}
