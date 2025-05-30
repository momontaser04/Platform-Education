using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }
     
    }
}
