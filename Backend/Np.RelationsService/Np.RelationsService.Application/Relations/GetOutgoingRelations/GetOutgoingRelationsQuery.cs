
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.GetOutgoingRelations;

public sealed record GetOutgoingRelationsQuery(Guid NoteId) : IQuery<GetOutgoingRelationsResponse>;