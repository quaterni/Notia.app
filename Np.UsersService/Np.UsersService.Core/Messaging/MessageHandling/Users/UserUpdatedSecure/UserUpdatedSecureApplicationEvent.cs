using Np.UsersService.Core.Authentication.Models;

namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserUpdatedSecure;

public sealed record UserUpdatedSecureApplicationEvent(string IdentityId, UserUpdateRepresentation UpdateRepresentation) : IApplicationEvent;
