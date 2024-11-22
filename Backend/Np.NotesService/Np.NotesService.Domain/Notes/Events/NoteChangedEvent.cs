using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Domain.Notes.Events;

public sealed record NoteChangedEvent(Guid NoteId) : IDomainEvent;
