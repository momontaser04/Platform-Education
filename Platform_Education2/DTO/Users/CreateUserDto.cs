using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Users
{
    public class CreateUserDto
    {

       
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
   
        public string Email { get; set; }
        public string Password { get; set; }
        public IList<string> Roles { get; set; }
    }
}
