
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.RegisterUser;

public class RegisterUserErrors
{
    public static Error EmailExists => new("RegisterUser:EmailExists", "Email was taken by another user");

    public static Error UsernameExists => new("RegisterUser:UsernameExists", "Username was taken by another user");
}
