using Microsoft.Extensions.Options;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using Np.UsersService.Core.Authentication.Models;
using System.Threading;

namespace Np.UsersService.Core.Authentication.Keycloak;

public class KeycloakTokenService : ITokenService
{
    private readonly HttpClient _httpClient;

    private readonly AuthClientOptions _authClientOptions;

    public KeycloakTokenService(
        HttpClient httpClient,
        IOptions<AuthClientOptions> options)
    {
        _httpClient = httpClient;
        _authClientOptions = options.Value;
    }

    public async Task<AuthorizationToken> GetTokenByRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        const string RefreshTokenParameter = "refresh_token";

        var requestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _authClientOptions.ClientId),
            new("client_secret", _authClientOptions.ClientSecret),
            new("grant_type", RefreshTokenParameter),
            new(RefreshTokenParameter, refreshToken),
        };

        var requestContent = new FormUrlEncodedContent(requestParameters);
        return await GetTokenFromRequest(requestContent, cancellationToken);
    }

    public async Task<AuthorizationToken> GetTokenByUserCredentials(UserCredentials userCredentials, CancellationToken cancellationToken)
    {
        const string PasswordGrantType = "password";

        var requestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _authClientOptions.ClientId),
            new("client_secret", _authClientOptions.ClientSecret),
            new("grant_type", PasswordGrantType),
            new("username", userCredentials.Username),
            new("password", userCredentials.Password)
        };

        var requestContent = new FormUrlEncodedContent(requestParameters);
        return await GetTokenFromRequest(requestContent, cancellationToken);
    }

    private async Task<AuthorizationToken> GetTokenFromRequest(FormUrlEncodedContent requestContent, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _authClientOptions.TokenUrl)
        {
            Content = requestContent
        };

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<AuthorizationToken>() ?? throw new ApplicationException("Http content was null");

        return token;
    }
}
