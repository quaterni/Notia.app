namespace Np.NotesService.Application.Relations.Service;

public sealed record RelationResponse(Guid Id, Guid OutgoingNoteId, Guid IncomingNoteId);