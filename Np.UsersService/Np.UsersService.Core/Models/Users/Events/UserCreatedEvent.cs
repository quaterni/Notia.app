namespace Np.UsersService.Core.Models.Users.Events;

public sealed record UserCreatedEvent(Guid Id) : IDomainEvent;
