using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Business.Abstractions;

public class UserRequest 
{
    protected UserRequest(string identityId)
    {
        IdentityId = identityId;
    }

    public string IdentityId { get; }

    public User? User { get; set; }
}

