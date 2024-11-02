using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;

namespace Np.UsersService.Core.Messaging.ModelEvents.Users;

public sealed record UserUpdatedEvent(Guid Id) : IModelEvent;
