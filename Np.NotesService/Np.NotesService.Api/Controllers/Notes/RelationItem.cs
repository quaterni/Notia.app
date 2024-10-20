namespace Np.NotesService.Api.Controllers.Notes;

public sealed record RelationItem(Guid Id, NoteItem OutgoingNote, NoteItem IncomingNote);
