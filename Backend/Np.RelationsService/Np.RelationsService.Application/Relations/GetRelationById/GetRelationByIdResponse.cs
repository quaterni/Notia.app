
namespace Np.RelationsService.Application.Relations.GetRelationById;

public record GetRelationByIdResponse(Guid RelationId, Guid IncomingId, Guid OutgoingId);
