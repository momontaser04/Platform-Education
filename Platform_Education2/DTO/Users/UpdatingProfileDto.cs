using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class UpdatingProfileDto
    {
        [Required,MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }
        [Required, MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }
        [Required, MaxLength(11, ErrorMessage = "PhoneNumber cannot exceed 11")]
        public string PhoneNumber { get; set; }
    }
}
