
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Relations.GetRelationByNotes;

public static class GetRelationByNotesErrors
{
    public static Error NotFound => new Error("[GetRelationByNotes:NotFound] Relation not found");
    public static Error BadRequest => new Error("[GetRelationByNotes:BadRequest] Bad request");
}
