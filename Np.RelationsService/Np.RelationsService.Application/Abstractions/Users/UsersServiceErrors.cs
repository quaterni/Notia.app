
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Abstractions.Users;

public class UsersServiceErrors
{
    public static Error NotFound => new("[UsersService:NotFound] User not found");
}
