using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Course
{
    public class CourseDtoWrite
    {
        [Required(ErrorMessage = "اسم الكورس مطلوب")]
        [MinLength(3, ErrorMessage = "يجب أن يكون اسم الكورس على الأقل 3 أحرف")]
        public string CourseName { get; set; }

        [Required(ErrorMessage ="سعر الكورس مطلوب")]
        public double SalesPrice { get; set; }
        [Required(ErrorMessage = "صوره الكورس مطلوبه")]
        public IFormFile Imagefile { get; set; }

        [Required(ErrorMessage = "وصف الكورس مطلوب")]
        [MinLength(10, ErrorMessage = "يجب أن يكون الوصف على الأقل 10 أحرف")]
        public string Description { get; set; }
    }
}
