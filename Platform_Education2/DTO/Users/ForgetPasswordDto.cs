using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
       public string Email { get; set; }
    }
}
