using FastEndpoints;
using FluentValidation;
using RateMyPet.Core;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostValidator : Validator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(Post.DescriptionMaxLength);

        RuleFor(request => request.SpeciesId)
            .NotEmpty();

        RuleFor(request => request.Tags)
            .Must(tags => tags.Count <= Post.TagsMaxCount)
            .WithMessage($"Maximum of {Post.TagsMaxCount} tags allowed")
            .ForEach(tag =>
            {
                tag.MinimumLength(Post.TagMinLength);
                tag.MaximumLength(Post.TagMaxLength);
            });
    }
}
