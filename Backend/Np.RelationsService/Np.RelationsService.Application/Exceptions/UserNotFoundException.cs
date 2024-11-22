
namespace Np.RelationsService.Application.Exceptions;

internal class UserNotFoundException : Exception
{
    public UserNotFoundException(string identityId) : base("User not found")
    {
        IdentityId = identityId;
    }

    public string IdentityId { get; }
}
