namespace Np.UsersService.Core.Authentication.Keycloak.Options;

public class AuthClientOptions
{
    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string TokenUrl { get; init; } = string.Empty;
}
