using RateMyPet.Core;
using RateMyPet.Infrastructure;

namespace RateMyPet.Api.Extensions;

public static class PostExtensions
{
    public static Uri? GetImageUrl(this Post post, HttpRequest? request)
    {
        if (request is null)
        {
            return null;
        }

        var host = request.Host;
        var port = host.Port is 5443 ? ":5443" : string.Empty;

        return new Uri($"https://{host.Host}{port}/{BlobContainerNames.Images}/{post.Id}");
    }
}
