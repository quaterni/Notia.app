using Np.UsersService.Core.Messaging.MessageHandling;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;

namespace Np.UsersService.Core.Messaging.ModelEvents.Users;

public sealed record UserCreatedSecureEvent(string Username, string Email, string Password, Guid Id) : IModelEvent;