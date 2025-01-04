using RateMyPet.Core;

namespace RateMyPet.Infrastructure.Extensions;

public static class PostExtensions
{
    public static string GetImagePath(this Post post) => $"/{BlobContainerNames.Images}/{post.Id}";
}
