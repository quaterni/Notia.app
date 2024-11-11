namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserDeleted;

public sealed record UserDeletedSecureApplicationEvent(string IdentityId) : IApplicationEvent;
