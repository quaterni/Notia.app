namespace Np.RelationsService.Api.Controllers.Relations;

public sealed record AddRelationRequest(Guid OutgoingNoteId, Guid IncomingNoteId);
