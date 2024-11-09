using MediatR;
using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserCreatedSecure;

public partial class AddUserToIdentityEventHandler : INotificationHandler<UserCreatedSecureApplicationEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AddUserToIdentityEventHandler> _logger;

    public AddUserToIdentityEventHandler(
        ApplicationDbContext dbContext, 
        IIdentityService identityService,
        ILogger<AddUserToIdentityEventHandler> logger)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(UserCreatedSecureApplicationEvent notification, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(notification.Id);
        if(user == null)
        {
            throw new ApplicationException("User not found in database, when attemping add user to identity");
        }
        if (user.IsSyncrhonizedWithIdentity)
        {
            LogAlreadySyncronized(_logger, user.Id);
            return;
        } 
        var resultIdentityId = await _identityService.CreateUserAsync(
            new CreateUserRequest(notification.Username, notification.Email, notification.Password), 
            cancellationToken);

        string identityId = resultIdentityId switch
        {
            _ when resultIdentityId.IsFailed && resultIdentityId.Error.Equals(IdentityErrors.UserExists) =>
                await GetUserFromIdentity(
                    notification.Username, 
                    notification.Email, 
                    cancellationToken),
            _ when resultIdentityId.IsSuccess => resultIdentityId.Value,
            _ => throw new UnhandledErrorException(resultIdentityId.Error)
        };

        user.IdentityId = identityId;
        user.IsSyncrhonizedWithIdentity = true;

        _dbContext.Update(user);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentException(ex);
        }

        LogUserSyncronized(_logger, user.Id);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User already syncronized, user id: {UserId}")]
    private static partial void LogAlreadySyncronized(ILogger logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Trying get user from identity: ({Username}, {Email})")]
    private static partial void LogTryingGetUserFromIdentity(ILogger logger, string username, string email);

    [LoggerMessage(Level = LogLevel.Information, Message = "User was syncronized: {UserId}")]
    private static partial void LogUserSyncronized(ILogger logger, Guid userId);

    private async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<string> GetUserFromIdentity(string username, string email, CancellationToken cancellationToken = default)
    {
        LogTryingGetUserFromIdentity(_logger, username, email);
        var resultUserView = await _identityService.GetUserByCredentialsAsync(username, email, cancellationToken);
        if (resultUserView.IsFailed)
        {
            throw new UnhandledErrorException(resultUserView.Error);
        }

        return resultUserView.Value.IdentityId;
    }
}
