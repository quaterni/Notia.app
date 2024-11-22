namespace Np.UsersService.Core.Authentication.Keycloak.Models;

public class UserRepresentation
{
    public required string Username { get; init; }

    public required string Email { get; init; }

    public bool EmailVerified { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public bool Enabled { get; init; }

    public List<CredentialRepresentation> Credentials { get; init; } = new();

}
