using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Models
{
    public class TbQuestions
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public virtual TbExam Exam { get; set; }

        public virtual ICollection<TbChoices> choices { get; set; }
    }
}
