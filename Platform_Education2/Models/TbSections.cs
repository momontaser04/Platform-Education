using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Models
{
    public class TbSections
    {
        [Key]
        public int Id { get; set; }
        public string SectionTitle { get; set; }

        [ForeignKey("courses")]
        public int CourseId { get; set; }
        public virtual TbCourses courses { get; set; }

        public virtual ICollection<TbVideoes> videoes { get; set; }

        public virtual TbExam exam { get; set; }
    }
}
