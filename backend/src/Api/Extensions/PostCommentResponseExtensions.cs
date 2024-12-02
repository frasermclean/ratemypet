using RateMyPet.Api.Endpoints.Posts;

namespace RateMyPet.Api.Extensions;

public static class PostCommentResponseExtensions
{
    public static IEnumerable<PostCommentResponse> ToCommentTree(this IEnumerable<PostCommentResponse> comments)
    {
        var dictionary = comments.ToDictionary(comment => comment.Id);

        foreach (var grouping in dictionary.Values.GroupBy(comment => comment.ParentId))
        {
            if (grouping.Key is null)
            {
                continue;
            }

            if (dictionary.TryGetValue(grouping.Key.Value, out var parentComment))
            {
                parentComment.Replies = grouping;
            }
        }

        return dictionary.Values.Where(comment => comment.ParentId is null);
    }
}
