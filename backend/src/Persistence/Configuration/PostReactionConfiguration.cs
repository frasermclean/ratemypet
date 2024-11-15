﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Persistence.Converters;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Persistence.Configuration;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions");

        builder.HasOne<Post>(postReaction => postReaction.Post)
            .WithMany(post => post.Reactions)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>(postReaction => postReaction.User)
            .WithMany(user => user.PostReactions)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(postReaction => postReaction.Reaction)
            .HasConversion<ReactionConverter>();
    }
}