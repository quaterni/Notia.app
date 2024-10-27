namespace Np.UsersService.Core.Models.Users.Events;

public sealed record UserUpdatedEvent(Guid Id) : IDomainEvent;
