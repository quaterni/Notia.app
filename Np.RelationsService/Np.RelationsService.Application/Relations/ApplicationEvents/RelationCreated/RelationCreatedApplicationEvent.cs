
using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.Relations.ApplicationEvents.RelationCreated;

public sealed record  RelationCreatedApplicationEvent(
    Guid RelationId, 
    Guid OutgoingNoteId, 
    Guid IncomingNoteId) : IApplicationEvent;