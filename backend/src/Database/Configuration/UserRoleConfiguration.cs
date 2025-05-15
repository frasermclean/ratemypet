using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasData(
            new UserRole
            {
                UserId = UserConfiguration.DeveloperUserId,
                RoleId = RoleConfiguration.ContributorId
            },
            new UserRole
            {
                UserId = UserConfiguration.DeveloperUserId,
                RoleId = RoleConfiguration.AdministratorId
            }
        );
    }
}
