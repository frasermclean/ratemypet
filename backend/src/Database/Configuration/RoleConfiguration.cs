using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasIndex(role => role.NormalizedName)
            .HasDatabaseName("IX_Roles_NormalizedName")
            .IsUnique();

        builder.HasData(
            new Role
            {
                Id = ContributorId,
                Name = Role.Contributor,
                NormalizedName = Role.Contributor.ToUpperInvariant()
            },
            new Role
            {
                Id = AdministratorId,
                Name = Role.Administrator,
                NormalizedName = Role.Administrator.ToUpperInvariant()
            }
        );
    }

    internal static Guid ContributorId = new("57c523c9-0957-4834-8fce-ff37fa861c36");
    internal static Guid AdministratorId = new("8e71eb35-2194-495b-b0e8-8690ebe7f918");
}
