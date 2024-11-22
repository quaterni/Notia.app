using System.Text.Json.Serialization;

namespace Np.UsersService.Core.Authentication.Models;

public class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
