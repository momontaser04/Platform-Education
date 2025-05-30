using Microsoft.AspNetCore.Identity;

namespace PlatformEduPro .Models
{
    public class AppUser:IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public bool IsDisabled { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }=[];


    }
}
