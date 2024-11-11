using MediatR;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Exceptions;

namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserDeleted;

public partial class UserDeletedSecureApplicationEventHandler : INotificationHandler<UserDeletedSecureApplicationEvent>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<UserDeletedSecureApplicationEventHandler> _logger;

    public UserDeletedSecureApplicationEventHandler(
        IIdentityService identityService,
        ILogger<UserDeletedSecureApplicationEventHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(UserDeletedSecureApplicationEvent notification, CancellationToken cancellationToken)
    {
        var result = await _identityService.RemoveUserAsync(notification.IdentityId, cancellationToken);

        if (result.IsFailed && result.Error.Equals(IdentityErrors.UserNotFound))
        {
            LogUserNotFound(_logger, notification.IdentityId);
        }
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }
        
        LogUserDelete(_logger, notification.IdentityId);
    }

    [LoggerMessage(Level=LogLevel.Information, Message="Trying delete user from identity service, but user not found (identity id: {IdentityId})")]
    private static partial void LogUserNotFound(ILogger logger, string identityId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User deleted from identity service (identity id: {IdentityId})")]
    private static partial void LogUserDelete(ILogger logger, string identityId);
}
