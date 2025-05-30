using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Video
{
    public class VideoDto
    {
        [Required(ErrorMessage ="من فضلك ادخل عنوان الفيديو")]
        public string VideoTitle { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل الفيديو")]
        public IFormFile VideoFile { get; set; }
        [Required]
        public int SectionId { get; set; }


    }
}
