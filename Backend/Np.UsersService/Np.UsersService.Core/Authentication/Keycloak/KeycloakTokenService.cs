using Microsoft.Extensions.Options;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Shared;
using System.Net;

namespace Np.UsersService.Core.Authentication.Keycloak;

public partial class KeycloakTokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KeycloakTokenService> _logger;
    private readonly AuthClientOptions _authClientOptions;

    public KeycloakTokenService(
        HttpClient httpClient,
        IOptions<AuthClientOptions> options,
        ILogger<KeycloakTokenService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authClientOptions = options.Value;
    }

    public async Task<Result<AuthorizationToken>> GetTokenByRefreshToken(string refreshToken, CancellationToken cancellationToken = default)
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

    public async Task<Result<AuthorizationToken>> GetTokenByUserCredentials(UserCredentials userCredentials, CancellationToken cancellationToken = default)
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

    [LoggerMessage(Level = LogLevel.Information, Message ="Sending token reqeust to: {TokenUrl}")]
    private static partial void LogSendingTokenRequest(ILogger logger, string tokenUrl);

    private async Task<Result<AuthorizationToken>> GetTokenFromRequest(FormUrlEncodedContent requestContent, CancellationToken cancellationToken = default)
    {
        var tokenUrl = _authClientOptions.TokenUrl;
        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = requestContent
        };

        LogSendingTokenRequest(_logger, tokenUrl);
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode && response.StatusCode.Equals(HttpStatusCode.Unauthorized))
        {
            return Result.Failure<AuthorizationToken>(TokenErrors.UnauthorizedError);
        }

        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<AuthorizationToken>() ?? throw new ApplicationException("Http content was null");
        return token;
    }
}
