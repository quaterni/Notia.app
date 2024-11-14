
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Abstractions.Users;

public interface IUsersService
{
    Task<Result<Guid>> GetUserIdAsync(string identityId, CancellationToken cancellationToken = default);
}
