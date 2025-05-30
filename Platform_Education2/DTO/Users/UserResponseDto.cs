namespace PlatformEduPro.DTO.Users
{
    public class UserResponseDto
    {
       public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDisabled { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
