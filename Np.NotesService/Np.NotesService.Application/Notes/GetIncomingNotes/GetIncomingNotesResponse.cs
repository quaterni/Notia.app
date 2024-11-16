using Np.NotesService.Application.Dtos;

namespace Np.NotesService.Application.Notes.GetIncomingNotes;

public sealed record GetIncomingNotesResponse(IEnumerable<NoteItemDto> IncomingNotes);
