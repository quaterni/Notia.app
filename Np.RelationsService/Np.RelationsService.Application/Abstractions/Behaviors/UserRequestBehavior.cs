
using MediatR;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Abstractions.Users;
using Np.RelationsService.Application.Exceptions;
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Abstractions.Behaviors;


internal class UserRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : UserRequest where TResponse : Result
{
    private readonly IUsersService _usersService;

    public UserRequestBehavior(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await _usersService.GetUserIdAsync(request.IdentityId);

        if (result.IsFailed && result.Error.Equals(UsersServiceErrors.NotFound))
        {
            throw new UserNotFoundException(request.IdentityId);
        }
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }

        request.UserId = result.Value;
        return await next();
    }
}
