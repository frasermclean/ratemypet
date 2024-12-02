﻿using FastEndpoints;
using FluentValidation;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api.Endpoints.Posts.Comments;

public class AddPostCommentValidator : Validator<AddPostCommentRequest>
{
    public AddPostCommentValidator()
    {
        RuleFor(request => request.Content)
            .NotEmpty()
            .MaximumLength(PostComment.ContentMaxLength);
    }
}