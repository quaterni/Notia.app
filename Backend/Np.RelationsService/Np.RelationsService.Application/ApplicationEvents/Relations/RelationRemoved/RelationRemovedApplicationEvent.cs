using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.ApplicationEvents.Relations.RelationRemoved;

public record RelationRemovedApplicationEvent(
    Guid RelationId,
    Guid OutgoingNoteId,
    Guid IncomingNoteId) : IApplicationEvent;
