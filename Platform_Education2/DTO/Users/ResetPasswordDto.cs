using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string code { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
     
    }
}
