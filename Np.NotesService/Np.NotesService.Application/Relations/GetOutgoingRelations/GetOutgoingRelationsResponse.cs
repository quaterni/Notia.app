
using Np.NotesService.Application.Relations.Shared;

namespace Np.NotesService.Application.Relations.GetOutgoingRelations;

public sealed record GetOutgoingRelationsResponse(IEnumerable<RelationItem> Relations);

