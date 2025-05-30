using Microsoft.AspNetCore.Authorization;

namespace PlatformEduPro.Contracts.Filters
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
