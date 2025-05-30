using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Question
{
    public class QuestionDto
    {

        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int ExamId { get; set; }
      public List<AnswerDto> answers { get; set; }

    }
}
