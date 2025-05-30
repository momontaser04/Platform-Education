namespace PlatformEduPro.DTO.Roles
{
    public record RoleDetailedDto
    (
        string Id,
       string Name,
       bool IsDeleted,
       IEnumerable<string> Permissions
    );
}
