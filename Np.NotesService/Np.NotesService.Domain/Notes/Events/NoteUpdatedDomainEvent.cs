using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Domain.Notes.Events
{
    public sealed record NoteUpdatedDomainEvent(Guid NoteId) : IDomainEvent;
}
