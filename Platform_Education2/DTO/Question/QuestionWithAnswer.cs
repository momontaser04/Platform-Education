using PlatformEduPro.DTO;
using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Question
{
    public class QuestionWithAnswer
    {
        [Required(ErrorMessage ="من فضلك ادخل نص السؤال")]
        public string QuestionText { get; set; }
        [Required]
        public int ExamId {  get; set; }
        [Required]
        public List<AnswerDto> answers { get; set; }
    }
}
