
using Dapper;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;

namespace Np.RelationsService.Application.Relations.GetRelationById;

internal class GetRelationByIdQueryHandler : IQueryHandler<GetRelationByIdQuery, GetRelationByIdResponse>
{
    private readonly IRelationsRepository _relationsRepository;

    public GetRelationByIdQueryHandler(IRelationsRepository relationsRepository)
    {
        _relationsRepository = relationsRepository;
    }

    public async Task<Result<GetRelationByIdResponse>> Handle(GetRelationByIdQuery request, CancellationToken cancellationToken)
    {
        var relation = await _relationsRepository.GetRelationById(request.RelationId);
        if (relation == null || !relation.Incoming.UserId.Equals(request.UserId))
        { 
            return Result.Failed<GetRelationByIdResponse>(GetRelationByIdErrors.NotFound);
        }

        return new GetRelationByIdResponse(relation.Id, relation.Incoming.Id, relation.Outgoing.Id);
    }
}
