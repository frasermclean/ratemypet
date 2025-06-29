﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

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

        builder.Property(user => user.LastActivity)
            .HasPrecision(2)
            .HasColumnName("LastActivityUtc");

        builder.Property(user => user.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasMany(user => user.Roles)
            .WithMany(role => role.Users)
            .UsingEntity<UserRole>();

        builder.HasMany(user => user.Posts)
            .WithOne(post => post.User)
            .HasForeignKey(post => post.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.Activities)
            .WithOne(activity => activity.User)
            .HasForeignKey(activity => activity.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(user => user.NormalizedUserName)
            .HasDatabaseName("IX_Users_NormalizedUserName")
            .IsUnique();

        builder.HasIndex(user => user.NormalizedEmail)
            .HasDatabaseName("IX_Users_NormalizedEmail")
            .IsUnique();

        builder.HasData(
            new User
            {
                Id = DeveloperUserId,
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

    internal static Guid DeveloperUserId = new("fb8ad061-3a62-45f9-2202-08dd15f6fc85");
}
