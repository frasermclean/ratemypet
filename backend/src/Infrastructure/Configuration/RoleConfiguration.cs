using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;
using RateMyPet.Core.Security;
using RateMyPet.Infrastructure.Extensions;

namespace RateMyPet.Infrastructure.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasIndex(role => role.NormalizedName)
            .HasDatabaseName("IX_Roles_NormalizedName")
            .IsUnique();

        builder.HasData([
            Roles.User.ToRole(),
            Roles.Administrator.ToRole()
        ]);
    }
}
