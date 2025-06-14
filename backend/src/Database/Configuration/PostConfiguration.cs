using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Database.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(post => post.Slug)
            .HasMaxLength(Post.SlugMaxLength)
            .HasDefaultValueSql("CONCAT('post-', LOWER(CONVERT(varchar(36), NEWID())))");

        builder.Property(post => post.Title)
            .HasMaxLength(Post.TitleMaxLength);

        builder.Property(post => post.Description)
            .HasMaxLength(Post.DescriptionMaxLength);

        builder.HasOne(post => post.Species)
            .WithMany(species => species.Posts)
            .HasForeignKey(post => post.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(post => post.Activities)
            .WithOne(activity => activity.Post)
            .HasForeignKey(activity => activity.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(post => post.Image, imageBuilder =>
        {
            imageBuilder.Property(image => image.AssetId)
                .HasMaxLength(PostImage.AssetIdMaxLength)
                .HasColumnName("ImageAssetId");

            imageBuilder.Property(image => image.PublicId)
                .HasMaxLength(PostImage.PublicIdMaxLength)
                .HasColumnName("ImagePublicId");

            imageBuilder.Property(image => image.Width).HasColumnName("ImageWidth");
            imageBuilder.Property(image => image.Height).HasColumnName("ImageHeight");
            imageBuilder.Property(image => image.Size).HasColumnName("ImageSize");
        });

        builder.Property(post => post.CreatedAtUtc)
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(post => post.UpdatedAtUtc)
            .HasPrecision(2);

        builder.Property(post => post.DeletedAtUtc)
            .HasPrecision(2);

        builder.Property(post => post.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();

        builder.HasQueryFilter(post => post.DeletedAtUtc == null);

        builder.HasAlternateKey(post => post.Slug);

        builder.HasIndex(post => post.Status);
    }
}
