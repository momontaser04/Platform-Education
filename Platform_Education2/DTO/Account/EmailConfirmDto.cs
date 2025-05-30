using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Account
{
    public class EmailConfirmDto
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
