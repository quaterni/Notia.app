using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Queries.GetUser;

public sealed class GetUserQuery : UserRequest, IQuery<GetUserResponse>
{
    public GetUserQuery(string identityId) : base(identityId)
    {       
    }
}
