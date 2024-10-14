
using Np.NotesService.Application.Shared;

namespace Np.NotesService.Application.Relations.Shared;

public sealed record RelationItem(Guid Id, NoteItem OutgoingItem, NoteItem IncomingNote);
