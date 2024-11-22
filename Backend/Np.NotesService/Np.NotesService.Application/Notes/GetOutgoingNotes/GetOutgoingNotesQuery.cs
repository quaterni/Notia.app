using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.GetOutgoingNotes;

public sealed record GetOutgoingNotesQuery(Guid NoteId, string IdentityId) : UserRequest(IdentityId), IQuery<GetOutgoingNotesResponse>;
