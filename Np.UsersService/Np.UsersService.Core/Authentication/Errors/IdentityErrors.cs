using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Authentication.Errors;

public static class IdentityErrors
{
    public static Error UserExists => new Error("Identity:UserExists", "User with same username or email exists");

    public static Error UserNotFound => new Error("Identity:UserNotFound", "User with given credentials not found");
}
