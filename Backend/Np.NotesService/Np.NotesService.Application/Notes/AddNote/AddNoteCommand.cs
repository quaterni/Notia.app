using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.AddNote
{
    public sealed record AddNoteCommand(string Data, string IdentityId) : UserRequest(IdentityId), ICommand<Guid>;
}
