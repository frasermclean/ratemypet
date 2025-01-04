using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateMyPet.Core;

namespace RateMyPet.Infrastructure.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(post => post.Title)
            .HasMaxLength(Post.TitleMaxLength);

        builder.Property(post => post.Description)
            .HasMaxLength(Post.DescriptionMaxLength);

        builder.OwnsOne(post => post.Image, imageBuilder =>
        {
            imageBuilder.Property(image => image.FileName)
                .HasMaxLength(PostImage.FileNameMaxLength)
                .HasColumnName("ImageFileName");

            imageBuilder.Property(image => image.MimeType)
                .HasMaxLength(PostImage.MimeTypeMaxLength)
                .HasColumnName("ImageMimeType");

            imageBuilder.Property(image => image.Width).HasColumnName("ImageWidth");
            imageBuilder.Property(image => image.Height).HasColumnName("ImageHeight");
            imageBuilder.Property(image => image.Size).HasColumnName("ImageSize");
            imageBuilder.Property(image => image.IsProcessed).HasColumnName("ImageIsProcessed");
        });

        builder.Property(post => post.CreatedAtUtc)
            .HasPrecision(2)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(post => post.UpdatedAtUtc)
            .HasPrecision(2);

        builder.Property(post => post.RowVersion)
            .IsRowVersion()
            .HasConversion<byte[]>();
    }
}
