
using Dapper;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Relations.Service;
using Np.NotesService.Application.Dtos;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Application.Dtos;
using Np.NotesService.Application.Relations.GetIncomingRelations;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Relations.GetOutgoingRelations;

internal class GetOutgoingRelationsQueryHandler : IQueryHandler<GetOutgoingRelationsQuery, GetOutgoingRelationsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IRelationsService _relationsService;

    public GetOutgoingRelationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IRelationsService relationsService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _relationsService = relationsService;
    }

    public async Task<Result<GetOutgoingRelationsResponse>> Handle(GetOutgoingRelationsQuery request, CancellationToken cancellationToken)
    {
        var noteId = request.NoteId;
        if (!await IsNoteContains(request.NoteId, cancellationToken))
        {
            return Result.Failure<GetOutgoingRelationsResponse>(NoteErrors.NotFound);
        }

        var outgoingNoteId = request.NoteId;
        IEnumerable<RelationResponse> rawRelations;
        try
        {
            rawRelations = await _relationsService.GetOutgoingRelations(outgoingNoteId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ServiceRequestException("Exception has thrown from relations service", ex);
        }
        if (rawRelations.Count() == 0)
        {
            return new GetOutgoingRelationsResponse(new List<RelationItem>());
        }

        var relations = await GetDataToRelationItems(outgoingNoteId, rawRelations);

        return new GetOutgoingRelationsResponse(relations);
    }


    private async Task<bool> IsNoteContains(Guid incomingNoteId, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        return (await connection.QueryAsync("SELECT * FROM notes WHERE id=@Id", new { Id = incomingNoteId })).Any();
    }

    private async Task<IEnumerable<RelationItem>> GetDataToRelationItems(Guid outgoingNoteId, IEnumerable<RelationResponse> rawRelations)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var responseDictionary = rawRelations.ToDictionary(r => r.IncomingNoteId);

        var dbOutgoingNoteResponse = await connection.QueryFirstAsync(
            "SELECT title, id FROM notes WHERE id=@Id", new { Id = outgoingNoteId });
        var outgoingNote = new NoteItemDto(
            dbOutgoingNoteResponse.title,
            dbOutgoingNoteResponse.id);

        var dbIncomingNotesResponse = await connection.QueryAsync(
            "SELECT title, id FROM notes WHERE id=ANY(@Ids)",
            new { Ids = rawRelations.Select(r => r.IncomingNoteId).ToList() });

        List<RelationItem> relations = new();

        foreach (var dynamicIncomingNote in dbIncomingNotesResponse)
        {
            var rawRelation = responseDictionary[(Guid)dynamicIncomingNote.id];

            var incomingNote = new NoteItemDto(
                dynamicIncomingNote.title,
                dynamicIncomingNote.id);

            relations.Add(new RelationItem(
                rawRelation.Id,
                outgoingNote,
                incomingNote));
        }
        return relations;
    }
}
