using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.ApplicationEvents.Notes.NoteCreated;

public record NoteCreatedApplicationEvent(Guid NoteId, Guid UserId) : IApplicationEvent;
