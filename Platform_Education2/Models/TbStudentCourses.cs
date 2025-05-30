using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Models
{
    public class TbStudentCourses
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("course")]
        public int CourseId { get; set; }
        public virtual TbCourses course { get; set; }


    }
}
