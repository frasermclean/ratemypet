﻿using System.Security.Claims;

namespace RateMyPet.Api.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_WithValidClaim_ShouldReturnExpectedUserId()
    {
        // arrange
        var expectedUserId = Guid.NewGuid();
        var principal = CreateClaimsPrincipal(new Claim(ClaimTypes.NameIdentifier, expectedUserId.ToString()));

        // act
        var returnedUserId = principal.GetUserId();

        // assert
        returnedUserId.ShouldBe(expectedUserId);
    }

    [Fact]
    public void GetUserId_WithNoClaim_ShouldReturnNull()
    {
        // arrange
        var principal = CreateClaimsPrincipal();

        // act
        var returnedUserId = principal.GetUserId();

        // assert
        returnedUserId.ShouldBeNull();
    }

    [Fact]
    public void GetUserId_WithInvalidClaim_ShouldReturnNull()
    {
        // arrange
        var principal = CreateClaimsPrincipal(new Claim(ClaimTypes.NameIdentifier, "invalid"));

        // act
        var returnedUserId = principal.GetUserId();

        // assert
        returnedUserId.ShouldBeNull();
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
}
