using FastEndpoints;
using FluentValidation;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentValidator : Validator<AddPostCommentRequest>
{
    public AddPostCommentValidator()
    {
        RuleFor(request => request.Content)
            .NotEmpty()
            .MaximumLength(PostComment.ContentMaxLength)
            .WithMessage("Content must not be empty and must not exceed {MaxLength} characters.");
    }
}
