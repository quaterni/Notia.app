using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Np.UsersService.Core.Authentication.Keycloak.Models;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using System.Net.Http.Headers;

namespace Np.UsersService.Core.Authentication.Keycloak;

public class KeycloakIdentityDelegatingHandler : DelegatingHandler
{
    private readonly IdentityClientOptions _identityClientOptions;

    public KeycloakIdentityDelegatingHandler(IOptions<IdentityClientOptions> options)
    {
        _identityClientOptions = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            accessToken.Value);

        var response = await base.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return response;
    }

    private async Task<KeycloakAccessToken> GetAccessToken(CancellationToken cancellationToken)
    {
        const string ClientCredentialsGrandType = "client_credentials";

        var authorizationReqeustParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _identityClientOptions.ClientId),
            new("client_secret", _identityClientOptions.ClientSecret),
            new("grant_type", ClientCredentialsGrandType)
        };

        var requestContent = new FormUrlEncodedContent(authorizationReqeustParameters);

        var reqeust = new HttpRequestMessage(
            HttpMethod.Post, 
            new Uri(_identityClientOptions.TokenUrl)) 
        { 
            Content = requestContent 
        };

        var response = await base.SendAsync(reqeust, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<KeycloakAccessToken>() ?? 
            throw new ApplicationException("Response content null");
    }
}
