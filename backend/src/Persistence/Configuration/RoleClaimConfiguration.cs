using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core.Security;

namespace RateMyPet.Persistence.Configuration;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("RoleClaims");

        builder.HasData(
            new IdentityRoleClaim<Guid>
            {
                Id = 1,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.Add
            }, new IdentityRoleClaim<Guid>
            {
                Id = 2,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.EditOwned
            }, new IdentityRoleClaim<Guid>
            {
                Id = 3,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.DeleteOwned
            }
        );
    }
}
