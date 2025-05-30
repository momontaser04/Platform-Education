using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Models
{

    public class TbCourses
    {

        [Key]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public double SalesPrice { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.UtcNow;
        public bool IsNotificationSent { get; set; } = false;
        public virtual TbImages images { get; set; }

        public virtual ICollection<TbSections> sections { get; set; }
        public virtual ICollection<TbStudentCourses> Courses { get; set; }
    }
}
