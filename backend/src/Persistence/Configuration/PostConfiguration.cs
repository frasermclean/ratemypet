using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Persistence.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(post => post.Id)
            .HasDefaultValueSql("newid()");

        builder.Property(post => post.Title)
            .HasMaxLength(Post.TitleMaxLength);

        builder.Property(post => post.Caption)
            .HasMaxLength(Post.CaptionMaxLength);

        builder.OwnsOne<PostReactions>(post => post.Reactions, navigationBuilder =>
        {
            navigationBuilder.Property(reactions => reactions.LikeCount).HasColumnName("LikeCount");
            navigationBuilder.Property(reactions => reactions.FunnyCount).HasColumnName("FunnyCount");
            navigationBuilder.Property(reactions => reactions.CrazyCount).HasColumnName("CrazyCount");
            navigationBuilder.Property(reactions => reactions.WowCount).HasColumnName("WowCount");
            navigationBuilder.Property(reactions => reactions.SadCount).HasColumnName("SadCount");
        });
    }
}