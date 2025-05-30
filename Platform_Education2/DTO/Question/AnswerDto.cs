using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Question
{
    public class AnswerDto
    {
        [Required]
        public string ChoiceText { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
