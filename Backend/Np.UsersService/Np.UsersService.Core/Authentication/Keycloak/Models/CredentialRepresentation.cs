namespace Np.UsersService.Core.Authentication.Keycloak.Models;

public class CredentialRepresentation
{
    public required string Type { get; init; }

    public required string Value { get; init; }

    public bool Temporary { get; init; }
}