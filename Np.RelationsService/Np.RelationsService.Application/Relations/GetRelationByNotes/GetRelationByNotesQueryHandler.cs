
using Dapper;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;

namespace Np.RelationsService.Application.Relations.GetRelationByNotes;

internal class GetRelationByNotesQueryHandler : IQueryHandler<GetRelationByNotesQuery, GetRelationByNotesResponse>
{
    private readonly IRelationsRepository _relationsRepository;

    public GetRelationByNotesQueryHandler(
        IRelationsRepository relationsRepository)
    {
        _relationsRepository = relationsRepository;
    }

    public async Task<Result<GetRelationByNotesResponse>> Handle(GetRelationByNotesQuery request, CancellationToken cancellationToken)
    {
        var relation = await _relationsRepository.GetRelationByNotes(request.IncomingNoteId, request.OutgoingNoteId, cancellationToken);

        if (relation == null)
        {
            return Result.Failed<GetRelationByNotesResponse>(GetRelationByNotesErrors.NotFound);
        }
        if (!relation.Incoming.UserId.Equals(request.UserId))
        {
            return Result.Failed<GetRelationByNotesResponse>(GetRelationByNotesErrors.NotFound);
        }

        return new GetRelationByNotesResponse(relation.Id, relation.Incoming.Id, relation.Outgoing.Id);
    }
}
