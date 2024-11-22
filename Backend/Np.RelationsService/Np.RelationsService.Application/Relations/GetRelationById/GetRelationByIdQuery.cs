
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.GetRelationById;

public record GetRelationByIdQuery(Guid RelationId, string IdentityId) : UserRequest(IdentityId), IQuery<GetRelationByIdResponse>;
