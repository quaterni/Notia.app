using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Authentication.Abstractions;

public interface IIdentityService
{
    Task<Result<string>> CreateUserAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken=default);

    Task<Result> RemoveUserAsync(string identityId, CancellationToken cancellationToken=default);

    Task<UserView?> GetUserByIdAsync(string identityId, CancellationToken cancellationToken=default);

    Task<Result<UserView>> GetUserByCredentialsAsync(string username, string email, CancellationToken cancellationToken=default);

    Task<Result> UpdateUserDataAsync(string identityId, UserUpdateRepresentation updateRepresentation, CancellationToken cancellationToken=default);

    Task<Result> UpdateUserPassword(string identityId, string newPassword, CancellationToken cancellationToken=default);
}
