using Microsoft.AspNetCore.Authorization;

namespace PlatformEduPro.Contracts.Filters
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
