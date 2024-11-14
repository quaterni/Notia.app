
using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.Notes.Events.NoteAdded;

public record NoteCreatedApplicationEvent(Guid NoteId, Guid UserId) : IApplicationEvent;
