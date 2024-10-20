
using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Relations.GetOutgoingRelations;

public sealed record GetOutgoingRelationsQuery(Guid NoteId) : IQuery<GetOutgoingRelationsResponse>;
