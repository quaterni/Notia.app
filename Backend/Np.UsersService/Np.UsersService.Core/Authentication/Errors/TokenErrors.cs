using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Authentication.Errors;

public class TokenErrors
{
    public static Error UnauthorizedError => new("Token:Unauthorized", "Invalid user credentials");
}
