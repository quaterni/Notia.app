using Np.NotesService.Application.Dtos;

namespace Np.NotesService.Application.Notes.GetNotesFromRoot;

public sealed record GetNotesFromRootResponse(IEnumerable<NoteItemDto> Notes);