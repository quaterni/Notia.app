using Np.NotesService.Application.Shared;

namespace Np.NotesService.Application.Notes.GetNotesFromRoot;

public sealed record GetNotesFromRootResponse(IEnumerable<NoteItem> Notes);