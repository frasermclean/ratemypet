namespace RateMyPet.Api.Endpoints.Roles;

public class AddRoleRequest
{
    public required string Name { get; init; }
    public required IEnumerable<string> Permissions { get; init; }
    public bool AddCurrentUser { get; init; }
}
