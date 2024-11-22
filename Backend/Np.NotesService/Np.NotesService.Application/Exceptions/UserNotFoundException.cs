
namespace Np.NotesService.Application.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string identityId) : base($"User with identity id not found {identityId}.")
    {
        IdentityId = identityId;
    }

    public string IdentityId { get; }
}
