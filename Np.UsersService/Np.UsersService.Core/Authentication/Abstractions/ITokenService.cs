using Np.UsersService.Core.Authentication.Models;

namespace Np.UsersService.Core.Authentication.Abstractions;

public interface ITokenService
{
    Task<AuthorizationToken> GetTokenByUserCredentials(UserCredentials userCredentials, CancellationToken cancellationToken);

    Task<AuthorizationToken> GetTokenByRefreshToken(string refreshToken, CancellationToken cancellationToken);
}
