using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class UserProfileDto
    {
       public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }


    }
}
