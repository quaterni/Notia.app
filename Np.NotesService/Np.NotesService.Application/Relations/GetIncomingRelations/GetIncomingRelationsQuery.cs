
using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Relations.GetIncomingRelations;

public sealed record class GetIncomingRelationsQuery(Guid NoteId) : IQuery<GetIncomingRelationsResponse>;
