using FastEndpoints;
using FluentValidation;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostValidator : Validator<AddPostRequest>
{
    public AddPostValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(Post.TitleMaxLength);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(Post.DescriptionMaxLength);

        RuleFor(request => request.SpeciesId)
            .NotEmpty();

        RuleFor(request => request.Image)
            .NotNull()
            .Must(image => image.ContentType.StartsWith("image/"))
            .WithMessage("Must be an image file");
    }
}
