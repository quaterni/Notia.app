
using System.Text.Json.Serialization;

namespace Notia.Desctop.Services.Accounts.Http.Response;

internal class LoginSuccessResponse
{
    public TokenResponse Token { get; set; }
}

internal class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
