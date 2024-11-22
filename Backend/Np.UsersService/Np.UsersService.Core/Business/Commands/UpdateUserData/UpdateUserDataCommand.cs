using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.UpdateUserData;

public class UpdateUserDataCommand : UserRequest, ICommand
{
    public UpdateUserDataCommand(string identityId) : base(identityId)
    {
    }

    public string? Username { get; init; }
    public string? Email { get; init; }
}
