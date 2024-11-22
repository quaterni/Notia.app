using Np.NotesService.Application.Dtos;

namespace Np.NotesService.Application.Notes.GetOutgoingNotes;

public sealed record GetOutgoingNotesResponse(IEnumerable<NoteItemDto> OutgoingNotes);

