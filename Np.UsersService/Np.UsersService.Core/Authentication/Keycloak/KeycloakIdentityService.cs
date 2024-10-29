using Microsoft.Extensions.Options;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using Np.UsersService.Core.Authentication.Keycloak.Models;
using Np.UsersService.Core.Dtos.Users;

namespace Np.UsersService.Core.Authentication.Keycloak;

public class KeycloakIdentityService : IIdentityService
{
    private const string PasswordCredentialType = "password";

    private readonly HttpClient _httpClient;

    private readonly IdentityClientOptions _identityClientOptions;

    public KeycloakIdentityService(HttpClient httpClient, IOptions<IdentityClientOptions> options)
    {
        _httpClient = httpClient;
        _identityClientOptions = options.Value;
    }

    public async Task<string> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var userRepresentation = CreateUserRepresentation(createUserRequest);

        var response = await _httpClient.PostAsJsonAsync(
            new Uri(_identityClientOptions.RealmUsersManagementUrl), 
            userRepresentation, 
            cancellationToken);

        return ExtreactIdentityIdFromHttpResponse(response);
    }

    public async Task RemoveUserAsync(string identityId, CancellationToken cancellationToken)
    {
        var url = new Uri($"{_identityClientOptions.RealmUsersManagementUrl}/{identityId}");

        await _httpClient.DeleteAsync(url, cancellationToken);
    }

    private UserRepresentation CreateUserRepresentation(CreateUserRequest createUserRequest)
    {
        var _userCreationOptions = _identityClientOptions.RealmUserCreationOptions;

        return new UserRepresentation()
        {
            Email = createUserRequest.Email,
            Username = createUserRequest.Username,
            EmailVerified = _userCreationOptions.EmailVerified,
            Enabled = _userCreationOptions.Enabled,
            FirstName = _userCreationOptions.FirstNamePlaceholder,
            LastName = _userCreationOptions.LastNamePlaceholder,
            Credentials = [
                new CredentialRepresentation()
                {
                    Temporary = _userCreationOptions.TemporaryCredentials,
                    Type = PasswordCredentialType,
                    Value = createUserRequest.Password
                }
            ]
        };
    }

    private static string ExtreactIdentityIdFromHttpResponse(HttpResponseMessage response)
    {
        const string usersSegmentName = "users/";

        var pathFromLocation = response.Headers.Location?.PathAndQuery;

        if (pathFromLocation == null)
        {
            throw new InvalidOperationException("Location header was null");
        }

        var index = pathFromLocation.IndexOf(
            usersSegmentName, 
            StringComparison.InvariantCultureIgnoreCase);

        var userIdentityId = pathFromLocation.Substring(index + usersSegmentName.Length);

        return userIdentityId;
    }


}
