
namespace Np.RelationsService.Application.Relations.GetRelationByNotes;

public record GetRelationByNotesResponse(Guid RelationsId, Guid IncomingNoteId, Guid OutgoingNoteId);
