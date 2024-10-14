
using Np.NotesService.Application.Relations.Shared;

namespace Np.NotesService.Application.Relations.GetIncomingRelations;

public sealed record  GetIncomingRelationsResponse(IEnumerable<RelationItem> Relations);
