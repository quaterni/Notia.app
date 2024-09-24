using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.UpdateNote
{
    public sealed record  UpdateNoteCommand(string Data, Guid NoteId) : ICommand;
}
