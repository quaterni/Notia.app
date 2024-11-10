using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Queries.GetUser;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserResponse>
{
    public Task<Result<GetUserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = request.User!;

        return Task.FromResult<Result<GetUserResponse>>(
            new GetUserResponse(user.Username, user.Email, user.IdentityId!));
    }
}
