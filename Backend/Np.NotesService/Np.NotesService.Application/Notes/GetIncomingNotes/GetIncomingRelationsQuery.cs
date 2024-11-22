using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.GetIncomingNotes;

public sealed record class GetIncomingRelationsQuery(Guid NoteId, string IdentityId) : UserRequest(IdentityId), IQuery<GetIncomingNotesResponse>;
