using System.Security.Claims;
using RateMyPet.Api.Extensions;
using RateMyPet.Core;

namespace RateMyPet.Api.Tests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void GetUserId_WithValidClaim_ShouldReturnExpectedUserId()
    {
        // arrange
        var expectedUserId = Guid.NewGuid();
        var principal = CreateClaimsPrincipal(expectedUserId);

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
    public void GetUserName_WithValidClaim_ShouldReturnExpectedUserName()
    {
        // arrange
        const string expectedUserName = "testuser";
        var principal = CreateClaimsPrincipal(userName: expectedUserName);

        // act
        var returnedUserName = principal.GetUserName();

        // assert
        returnedUserName.ShouldBe(expectedUserName);
    }

    [Fact]
    public void GetUserName_WithNoClaim_ShouldReturnNull()
    {
        // arrange
        var principal = CreateClaimsPrincipal();

        // act
        var returnedUserName = principal.GetUserName();

        // assert
        returnedUserName.ShouldBeNull();
    }

    [Fact]
    public void GetEmailAddress_WithValidClaim_ShouldReturnExpectedEmailAddress()
    {
        // arrange
        const string expectedEmailAddress = "test@example.com";
        var principal = CreateClaimsPrincipal(emailAddress: expectedEmailAddress);

        // act
        var returnedEmailAddress = principal.GetEmailAddress();

        // assert
        returnedEmailAddress.ShouldBe(expectedEmailAddress);
    }

    [Fact]
    public void GetEmailAddress_WithNoClaim_ShouldReturnNull()
    {
        // arrange
        var principal = CreateClaimsPrincipal();

        // act
        var returnedEmailAddress = principal.GetEmailAddress();

        // assert
        returnedEmailAddress.ShouldBeNull();
    }

    [Fact]
    public void GetRoles_WithMultipleRoleClaims_ShouldReturnAllRoles()
    {
        // arrange
        var roles = new[] { Role.Contributor, Role.Administrator };
        var principal = CreateClaimsPrincipal(roles: roles);


        // act
        var returnedRoles = principal.GetRoles();

        // assert
        returnedRoles.ShouldBe(roles);
    }

    [Fact]
    public void GetRoles_WithNoRoleClaims_ShouldReturnEmptyEnumerable()
    {
        // arrange
        var principal = CreateClaimsPrincipal();

        // act
        var returnedRoles = principal.GetRoles();

        // assert
        returnedRoles.ShouldBeEmpty();
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(
        Guid? userId = null, string?
            userName = null,
        string? emailAddress = null,
        IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>();

        if (userId is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
        }

        if (userName is not null)
        {
            claims.Add(new Claim(ClaimTypes.Name, userName));
        }

        if (emailAddress is not null)
        {
            claims.Add(new Claim(ClaimTypes.Email, emailAddress));
        }

        if (roles is not null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var identity = new ClaimsIdentity(claims);
        return new ClaimsPrincipal(identity);
    }
}
