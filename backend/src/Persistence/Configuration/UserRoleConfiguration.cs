using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;
using RateMyPet.Core.Security;

namespace RateMyPet.Persistence.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasData(
            new UserRole
            {
                UserId = new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                RoleId = Roles.User.Id
            },
            new UserRole
            {
                UserId = new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                RoleId = Roles.Administrator.Id
            }
        );
    }
}
