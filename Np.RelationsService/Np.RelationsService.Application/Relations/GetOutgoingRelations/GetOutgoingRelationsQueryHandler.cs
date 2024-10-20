
using Dapper;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Relations.Shared;
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Relations.GetOutgoingRelations;

internal class GetOutgoingRelationsQueryHandler : IQueryHandler<GetOutgoingRelationsQuery, GetOutgoingRelationsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOutgoingRelationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetOutgoingRelationsResponse>> Handle(GetOutgoingRelationsQuery request, CancellationToken cancellationToken)
    {
        var outgoingNoteId = request.NoteId;
        using var connection = _sqlConnectionFactory.CreateConnection();

        var dbResponse = await connection.QueryAsync(
            "SELECT id, incoming_id FROM relations WHERE outgoing_id=@OutgoingID", 
            new { OutgoingId = outgoingNoteId});

        var relations = dbResponse
            .Select(x => new RelationItem(x.id, outgoingNoteId, x.incoming_id));

        return new GetOutgoingRelationsResponse(relations);
    }
}
