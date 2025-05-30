using System.ComponentModel.DataAnnotations;

namespace PlatformEduPro.Extensions
{
    public class JwtOptions
    {
        [Required]
        public string Issuer {  get; init; }=string.Empty;
        [Required]
        public string Audience {  get; init; }=string.Empty;
        [Required]
        public string SecretKey {  get; init; }=string.Empty;
        [Range(1,30,ErrorMessage = "Invalid ExpiredMinutes ")]
        public int ExpiredMinutes {  get; init; }
    }
}
