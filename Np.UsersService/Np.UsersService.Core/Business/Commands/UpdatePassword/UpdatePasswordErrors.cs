using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.UpdatePassword;

public static class UpdatePasswordErrors
{
    public static Error SamePasswordError => new("UpdatePassword:SamePassword", "New and old passwords are same");

    public static Error InvalidOldPasswordError => new("UpdatePassword:InvalidOldPassword", "Old password are invalid");
}
