
using Np.RelationsService.Application.Relations.Shared;

namespace Np.RelationsService.Application.Relations.GetOutgoingRelations;

public sealed record GetOutgoingRelationsResponse(IEnumerable<RelationItem> Relations);
