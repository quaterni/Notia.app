namespace Np.UsersService.Core.Authentication.Keycloak.Options;

public class IdentityClientOptions
{
    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string RealmUsersManagementUrl { get; init; } = string.Empty;

    public string TokenUrl { get; init; } = string.Empty;

    public required RealmUserCreationOptions RealmUserCreationOptions { get; init; }
}
