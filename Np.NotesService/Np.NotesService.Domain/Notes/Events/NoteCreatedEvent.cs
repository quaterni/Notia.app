using Np.NotesService.Domain.Abstractions;


namespace Np.NotesService.Domain.Notes.Events;

public sealed record NoteCreatedEvent(Guid NoteId, Guid UserId) : IDomainEvent;
