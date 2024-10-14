
using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.Relations.ApplicationEvents.RelationRemoved;

public record RelationRemovedApplicationEvent(
    Guid RelationId,
    Guid OutgoingNoteId,
    Guid IncomingNoteId) : IApplicationEvent;
