using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;
using RateMyPet.Core.Security;

namespace RateMyPet.Persistence.Configuration;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims");

        builder.HasData(
            new RoleClaim
            {
                Id = 1,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.Add
            }, new RoleClaim
            {
                Id = 2,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.EditOwned
            }, new RoleClaim
            {
                Id = 3,
                RoleId = Roles.User.Id,
                ClaimType = Claims.Permissions,
                ClaimValue = Permissions.Posts.DeleteOwned
            }
        );
    }
}
