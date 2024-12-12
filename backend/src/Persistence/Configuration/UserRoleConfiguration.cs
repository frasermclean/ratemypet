using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core.Security;

namespace RateMyPet.Persistence.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasData(
            new IdentityUserRole<Guid>()
            {
                UserId = new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                RoleId = Roles.User.Id
            },
            new IdentityUserRole<Guid>()
            {
                UserId = new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                RoleId = Roles.Administrator.Id
            }
        );
    }
}
