﻿using System.Security.Claims;
using FastEndpoints;

namespace RateMyPet.Api.Endpoints.Posts;

public class AddPostRequest
{
    public required string Title { get; init; }
    public required string Caption { get; init; }
    [FromClaim(ClaimTypes.NameIdentifier)] public Guid UserId { get; init; }
}