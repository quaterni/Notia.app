using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public class LogUserErrors
{
    public static Error UserUnathorized => new("LogUser:Unauthorized", "Username (email) or passward was incorrect");
}
