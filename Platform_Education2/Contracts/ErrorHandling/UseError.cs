using PlatformEduPro.Contracts.Authorization;

namespace PlatformEduPro.Contracts.Errors
{
    public static class UseError
    {
        public static readonly Error InvalidCredentials =
            new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedEmail =
            new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
        
        public static readonly Error InvalidCode =
            new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRole =
        new("User.InvalidRole", "InvalidRole", StatusCodes.Status401Unauthorized);

        public static readonly Error DuplicatedConfirmation =
            new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);


        public static readonly Error DisabledUser =
                new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);
        public static readonly Error LockedUser =
       new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

        public static readonly Error UserNotFound =
            new("User.UserNotFound", "UserNotFound", StatusCodes.Status400BadRequest);


    }
}
