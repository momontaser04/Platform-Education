namespace PlatformEduPro.Contracts.Authorization
{
    public record AuthResponse
    (

         string Id,
         string? Email,
         string FirstName,
         string LastName,
         string Token,
         int ExpiresIn,
         string RefresToken,
         DateTime RefreshTokenExpiration
    )
   ;
}
