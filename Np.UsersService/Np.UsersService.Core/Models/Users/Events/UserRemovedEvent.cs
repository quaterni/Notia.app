namespace Np.UsersService.Core.Models.Users.Events;

public sealed record UserRemovedEvent(Guid Id) : IDomainEvent;
