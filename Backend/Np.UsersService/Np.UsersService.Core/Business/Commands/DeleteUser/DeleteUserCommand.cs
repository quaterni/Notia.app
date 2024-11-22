using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.DeleteUser;

public class DeleteUserCommand : UserRequest, ICommand
{
    public DeleteUserCommand(string identityId) : base(identityId)
    {
    }
}
