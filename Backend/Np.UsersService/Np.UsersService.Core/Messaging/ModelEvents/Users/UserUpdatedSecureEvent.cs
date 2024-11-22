using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;

namespace Np.UsersService.Core.Messaging.ModelEvents.Users;

public sealed record UserUpdatedSecureEvent(string IdentityId, UserUpdateRepresentation UpdateRepresentation) : IModelEvent;
