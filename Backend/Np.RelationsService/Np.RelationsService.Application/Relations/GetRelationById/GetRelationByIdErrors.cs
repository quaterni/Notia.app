
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Relations.GetRelationById;

public static class GetRelationByIdErrors
{
    public static Error NotFound => new Error("[GetRelationById:NotFound] Relation not found");
}
