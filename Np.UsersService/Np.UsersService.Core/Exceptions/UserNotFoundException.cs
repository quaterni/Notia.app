
namespace Np.UsersService.Core.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string identityId) : base($"User not found by identity id {identityId}")
    {
        IdentityId = identityId;
    }

    public string IdentityId { get; }
}
