using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Models
{
    public class TbExam
    {
        [Key]
        public int Id { get; set; }

        

        public string Title { get; set; }
        [ForeignKey("Section")]
        public int SectionId { get; set; }
        public virtual TbSections Section { get; set; }

        public virtual ICollection<TbQuestions> Questions { get; set; }
    }
}
