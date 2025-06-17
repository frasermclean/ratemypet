using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;
using RateMyPet.Database.Converters;

namespace RateMyPet.Database.Configuration;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions");

        builder.HasOne<Post>(postReaction => postReaction.Post)
            .WithMany(post => post.Reactions)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>(postReaction => postReaction.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(postReaction => postReaction.Reaction)
            .HasConversion<ReactionConverter>();

        builder.Property(postReaction => postReaction.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasQueryFilter(postReaction => postReaction.Post.DeletedAtUtc == null);
    }
}
