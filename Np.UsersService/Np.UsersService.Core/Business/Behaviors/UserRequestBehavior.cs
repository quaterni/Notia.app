using MediatR;
using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Models.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Behaviors;

public partial class UserRequestBehavior<TRequest, TResponse> : IPipelineBehavior<UserRequest, TResponse>
    where TRequest : UserRequest 
    where TResponse : Result
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserRequestBehavior<TRequest, TResponse>> _logger;

    public UserRequestBehavior(ApplicationDbContext dbContext, ILogger<UserRequestBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TResponse> Handle(UserRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = await GetUser(request.IdentityId, cancellationToken)
            ?? throw new UserNotFoundException(request.IdentityId);

        request.User = user;

        LogUserFound(_logger, request.IdentityId);

        return await next();
    }

    [LoggerMessage(Level = LogLevel.Information, Message ="User found in users storage, identity id: {IdentityId}")]
    private static partial void LogUserFound(ILogger logger, string identityId);

    private async Task<User?> GetUser(string identityId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<User>()
            .Where(u => u.IdentityId != null? u.IdentityId.Equals(identityId) : false)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
