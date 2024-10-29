using System.Text.Json.Serialization;

namespace Np.UsersService.Core.Authentication.Keycloak.Models;

public class KeycloakAccessToken
{
    [JsonPropertyName("access_token")]
    public string Value { get; init; } = string.Empty;
}
