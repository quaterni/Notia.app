namespace Np.RelationsService.Api.Controllers.Relations;

public sealed record RelationRequest(Guid IncomingNoteId, Guid OutgoingNoteId);
