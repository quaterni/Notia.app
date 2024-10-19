
using Np.NotesService.Application.Dtos;

namespace Np.NotesService.Application.Relations.GetIncomingRelations;

public sealed record  GetIncomingRelationsResponse(IEnumerable<RelationItem> Relations);
