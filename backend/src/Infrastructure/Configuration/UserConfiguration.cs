using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.UserName)
            .HasMaxLength(User.UserNameMaxLength)
            .IsRequired();

        builder.Property(user => user.NormalizedUserName)
            .HasMaxLength(User.UserNameMaxLength)
            .IsRequired();

        builder.Property(user => user.Email)
            .IsRequired();

        builder.Property(user => user.NormalizedEmail)
            .IsRequired();

        builder.Property(user => user.LastSeen)
            .HasPrecision(2)
            .HasColumnName("LastSeenUtc");

        builder.Property(user => user.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<UserRole>();

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
                PasswordHash = "AQAAAAIAAYagAAAAEBoXY3pqPk5/R8T+lWoMQvgHwVaoG3hT+R8xD1iwrLk8S6Xy4F1vJbFEx6XQLShF5w==",
                SecurityStamp = "SECHFYM7WURIXXX2FGC33PN3RWDOU4YD",
                ConcurrencyStamp = "591b07b3-192b-410c-b31a-063163d5dc06"
            });
    }
}
