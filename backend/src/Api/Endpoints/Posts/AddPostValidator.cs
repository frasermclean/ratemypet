using FastEndpoints;
using FluentValidation;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostValidator : Validator<AddPostRequest>
{
    public AddPostValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(Post.TitleMaxLength);

        RuleFor(request => request.Caption)
            .NotEmpty()
            .MaximumLength(Post.CaptionMaxLength);

        RuleFor(request => request.Image)
            .NotNull()
            .Must(image => image.ContentType.StartsWith("image/"))
            .WithMessage("Must be an image file");
    }
}
