using Np.UsersService.Core.Dtos.Users;

namespace Np.UsersService.Core.Authentication.Abstractions;

public interface IIdentityService
{
    Task<string> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken);

    Task RemoveUserAsync(string identityId, CancellationToken cancellationToken);

    Task<UserView?> GetUserAsync(string identityId, CancellationToken cancellationToken);
}
