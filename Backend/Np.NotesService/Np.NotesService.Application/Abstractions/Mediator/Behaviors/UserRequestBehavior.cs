
using MediatR;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Users;
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Application.Abstractions.Mediator.Behaviors;

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

        if (result.IsFailed && result.Message.Equals(UsersServiceErrors.NotFound))
        {
            throw new UserNotFoundException(request.IdentityId);
        }
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Message);
        }

        request.UserId = result.Value;

        return await next();
    }
}
