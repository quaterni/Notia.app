
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Relations.RemoveRelationById;

public static class RemoveRelationErrors
{
    public static Error NotFound => new Error("[RemoveRelation:NotFound] Relation not found");
}
