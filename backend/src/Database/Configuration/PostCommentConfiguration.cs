using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.Property(comment => comment.Id)
            .HasDefaultValueSql("newid()");

        builder.HasOne<Post>(comment => comment.Post)
            .WithMany(post => post.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(comment => comment.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(comment => comment.Content)
            .HasMaxLength(PostComment.ContentMaxLength);

        builder.Property(comment => comment.CreatedAtUtc)
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(comment => comment.UpdatedAtUtc)
            .HasPrecision(2);

        builder.Property(comment => comment.DeletedAtUtc)
            .HasPrecision(2);

        builder.Property(comment => comment.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();
    }
}
