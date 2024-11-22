using MediatR;
using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserUpdatedSecure;

public partial class UpdateUserInIdentityEventHandler : INotificationHandler<UserUpdatedSecureApplicationEvent>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<UpdateUserInIdentityEventHandler> _logger;

    public UpdateUserInIdentityEventHandler(
        IIdentityService identityService,
        ILogger<UpdateUserInIdentityEventHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(UserUpdatedSecureApplicationEvent notification, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateUserDataAsync(notification.IdentityId, notification.UpdateRepresentation);

        if(result.IsFailed && result.Error.Equals(IdentityErrors.UserNotFound))
        {
            LogUserNotFound(_logger, notification.IdentityId);
            return;
        }
        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Updata data to identity failed, user not found, identity id: {IdentityId}")]
    private static partial void LogUserNotFound(ILogger logger, string identityId);
}
