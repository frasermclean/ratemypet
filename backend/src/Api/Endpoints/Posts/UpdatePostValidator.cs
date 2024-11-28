using FastEndpoints;
using FluentValidation;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts;

public class UpdatePostValidator : Validator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(Post.TitleMaxLength);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(Post.DescriptionMaxLength);

        RuleFor(request => request.SpeciesId)
            .NotEmpty();
    }
}
