using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Authentication.Abstractions;

public interface ITokenService
{
    Task<Result<AuthorizationToken>> GetTokenByUserCredentials(UserCredentials userCredentials, CancellationToken cancellationToken=default);

    Task<Result<AuthorizationToken>> GetTokenByRefreshToken(string refreshToken, CancellationToken cancellationToken = default);
}
