
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.GetIncomingRelations;

public sealed record GetIncomingRelationsQuery(Guid NoteId) : IQuery<GetIncomingRelationsResponse>;
