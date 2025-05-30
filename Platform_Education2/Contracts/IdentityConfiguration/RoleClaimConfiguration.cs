using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformEduPro.Contracts.Const;

namespace PlatformEduPro.Contracts.IdentityConfiguration
{

    public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            //Default Data
            var permissions = Permissions.GetAllPermissions();
            var adminClaims = new List<IdentityRoleClaim<string>>();

            for (var i = 0; i < permissions.Count; i++)
            {
                adminClaims.Add(new IdentityRoleClaim<string>
                {
                    Id = i + 1,
                    ClaimType = Permissions.Type,
                    ClaimValue = permissions[i],
                    RoleId = DefaultRoles.AdminRoleId
                });
            }

            // هنا لو انت عاوز تضيف صلاحيات تانية للادمن او انت تختار 
//            var adminPermissions = new List<string>
//{
//    Permissions.Course_Create,
//    Permissions.Course_Delete,
//    Permissions.Course_GetAll,
//    Permissions.GetUsers,
//    Permissions.AddUsers
//};

//            var memberPermissions = new List<string>
//{
//    Permissions.Course_GetAll,
//    Permissions.Course_GetById
//};

//            var adminClaims = new List<IdentityRoleClaim<string>>();
//            int claimId = 1;

//            foreach (var permission in adminPermissions)
//            {
//                adminClaims.Add(new IdentityRoleClaim<string>
//                {
//                    Id = claimId++,
//                    ClaimType = Permissions.Type,
//                    ClaimValue = permission,
//                    RoleId = DefaultRoles.AdminRoleId
//                });
//            }

//            foreach (var permission in memberPermissions)
//            {
//                adminClaims.Add(new IdentityRoleClaim<string>
//                {
//                    Id = claimId++,
//                    ClaimType = Permissions.Type,
//                    ClaimValue = permission,
//                    RoleId = DefaultRoles.MemberRoleId
//                });
//            }


            builder.HasData(adminClaims);
        }
    }
}
