
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Application.Users;

public interface IUsersService
{
    Task<Result<Guid>> GetUserIdAsync(string identityId);
}
