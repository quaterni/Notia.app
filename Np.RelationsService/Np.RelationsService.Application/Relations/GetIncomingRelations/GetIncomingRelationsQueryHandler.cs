
using Dapper;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Relations.Shared;
using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Application.Relations.GetIncomingRelations;

internal class GetIncomingRelationsQueryHandler : IQueryHandler<GetIncomingRelationsQuery, GetIncomingRelationsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetIncomingRelationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetIncomingRelationsResponse>> Handle(GetIncomingRelationsQuery request, CancellationToken cancellationToken)
    {
        var noteId = request.NoteId;
        using var connection = _sqlConnectionFactory.CreateConnection();

        var dbResponse = await connection.QueryAsync(
            "SELECT id, outgoing_id FROM relations WHERE incoming_id=@IncomingId",
            new { IncomingId = noteId });

        var relationItems = dbResponse.Select(x => new RelationItem(x.id, x.outgoing_id, noteId));

        return new GetIncomingRelationsResponse(relationItems);
    }
}
