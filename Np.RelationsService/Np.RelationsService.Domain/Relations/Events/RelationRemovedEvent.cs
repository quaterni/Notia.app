
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Domain.Relations.Events;

public sealed record RelationRemovedEvent(
    Guid RelationId,
    Guid OutgoingNoteId,
    Guid IncomingNoteId) : IDomainEvent;
