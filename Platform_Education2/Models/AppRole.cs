using Microsoft.AspNetCore.Identity;

namespace PlatformEduPro.Models
{
    public class AppRole:IdentityRole
    {
        public bool IsDefault { get; set; } 
        public bool IsDeleted { get; set; } 

    }
}
