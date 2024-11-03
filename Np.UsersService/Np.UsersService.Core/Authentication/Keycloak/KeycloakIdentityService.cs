using Microsoft.Extensions.Options;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using Np.UsersService.Core.Authentication.Keycloak.Models;
using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Shared;
using Np.UsersService.Core.Authentication.Errors;

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

    public async Task<Result<string>> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default)
    {
        var userRepresentation = CreateUserRepresentation(createUserRequest);

        var response = await _httpClient.PostAsJsonAsync(
            new Uri(_identityClientOptions.RealmUsersManagementUrl), 
            userRepresentation, 
            cancellationToken);

        if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            return Result.Failure<string>(IdentityErrors.UserExists);
        }

        return ExtreactIdentityIdFromHttpResponse(response);
    }

    public async Task RemoveUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var url = new Uri($"{_identityClientOptions.RealmUsersManagementUrl}/{identityId}");

        await _httpClient.DeleteAsync(url, cancellationToken);
    }

    public async Task<UserView?> GetUserByIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var url = new Uri($"{_identityClientOptions.RealmUsersManagementUrl}/{identityId}");

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserView>();
    }


    public async Task<Result<UserView>> GetUserByCredentialsAsync(string username, string email, CancellationToken cancellationToken=default)
    {
        var url = new Uri($"{_identityClientOptions.RealmUsersManagementUrl}?username={username}&email={email}");

        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var user = response.Content.ReadFromJsonAsAsyncEnumerable<UserView>()
            .ToBlockingEnumerable()
            .SingleOrDefault();

        if(user == null)
        {
            return Result.Failure<UserView>(IdentityErrors.UserNotFound);
        }

        return user;
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
