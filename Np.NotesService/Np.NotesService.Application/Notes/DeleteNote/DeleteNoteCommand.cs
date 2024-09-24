using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.DeleteNote
{
    public sealed record DeleteNoteCommand(Guid NoteId) : ICommand;
}
