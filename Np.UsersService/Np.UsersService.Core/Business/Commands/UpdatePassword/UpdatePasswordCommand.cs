using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.UpdatePassword;

public class UpdatePasswordCommand : UserRequest, ICommand
{
    public UpdatePasswordCommand(
        string identityId, 
        string oldPassword, 
        string newPassword) : base(identityId)
    {
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    public string OldPassword { get; }

    public string NewPassword { get; }
}
