using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.DTO.Account
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
