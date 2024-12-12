using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasIndex(user => user.NormalizedUserName)
            .HasDatabaseName("IX_Users_NormalizedUserName")
            .IsUnique();

        builder.HasIndex(user => user.NormalizedEmail)
            .HasDatabaseName("IX_Users_NormalizedEmail")
            .IsUnique();

        builder.HasData(
            new User
            {
                Id = new Guid("fb8ad061-3a62-45f9-2202-08dd15f6fc85"),
                UserName = "frasermclean",
                NormalizedUserName = "FRASERMCLEAN",
                Email = "dev@frasermclean.com",
                NormalizedEmail = "DEV@FRASERMCLEAN.COM",
                EmailConfirmed = true,
                ConcurrencyStamp = "initial",
                SecurityStamp = "initial",
            });
    }
}
