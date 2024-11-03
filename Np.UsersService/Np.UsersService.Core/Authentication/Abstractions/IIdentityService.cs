using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Authentication.Abstractions;

public interface IIdentityService
{
    Task<Result<string>> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken=default);

    Task RemoveUserAsync(string identityId, CancellationToken cancellationToken=default);

    Task<UserView?> GetUserByIdAsync(string identityId, CancellationToken cancellationToken=default);

    Task<Result<UserView>> GetUserByCredentialsAsync(string username, string email, CancellationToken cancellationToken=default);
}
