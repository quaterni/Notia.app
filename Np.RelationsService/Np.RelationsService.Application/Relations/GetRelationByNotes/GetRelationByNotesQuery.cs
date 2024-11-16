
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.GetRelationByNotes;

public sealed record GetRelationByNotesQuery(
    Guid IncomingNoteId, 
    Guid OutgoingNoteId, 
    string IdentityId) 
    : UserRequest(IdentityId), IQuery<GetRelationByNotesResponse>;
