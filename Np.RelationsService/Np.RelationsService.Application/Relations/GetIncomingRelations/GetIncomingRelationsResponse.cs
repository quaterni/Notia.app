
using Np.RelationsService.Application.Relations.Shared;

namespace Np.RelationsService.Application.Relations.GetIncomingRelations;

public sealed record  GetIncomingRelationsResponse(IEnumerable<RelationItem> Relations);
