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
        var port = host.Port is 443 or 80 ? "" : $":{host.Port}";

        return new Uri($"https://{host.Host}{port}/{BlobContainerNames.Images}/{post.Id}");
    }
}
