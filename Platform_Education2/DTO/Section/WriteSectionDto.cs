using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Section
{
    public class WriteSectionDto
    {
        [Required(ErrorMessage =" من فضلك ادخل اسم السكشن ")]
        public string SectionName { get; set; }
        [Required]
        public int CourseId { get; set; }

        [Required(ErrorMessage ="من فضلك ادخل عنوان الامتحان")]
        public string ExamTitle {  get; set; }
        


    }
}
