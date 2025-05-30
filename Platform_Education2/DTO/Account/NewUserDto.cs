using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PlatformEduPro.DTO.Account
{
    public class NewUserDto
    {
        [Required,MaxLength(20,ErrorMessage ="Please Enter FirstName Less than 200")]
        public string firstName { get; set; }

        [Required, MaxLength(20, ErrorMessage = "Please Enter LastName Less than 200")]
        public string lastName { get; set; }
        [EmailAddress(ErrorMessage ="Please Enter Valid Email")]
        public string Email { get; set; }
        [Required]
        public string password { get; set; }

        [Required, MaxLength(11, ErrorMessage = "Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }

    }
}
