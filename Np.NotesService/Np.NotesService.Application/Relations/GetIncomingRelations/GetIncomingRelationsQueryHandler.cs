 
using Dapper;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Relations.Service;
using Np.NotesService.Application.Dtos;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Relations.GetIncomingRelations;

internal class GetIncomingRelationsQueryHandler : IQueryHandler<GetIncomingRelationsQuery, GetIncomingRelationsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IRelationsService _relationsService;

    public GetIncomingRelationsQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IRelationsService relationsService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _relationsService = relationsService;
    }

    public async Task<Result<GetIncomingRelationsResponse>> Handle(GetIncomingRelationsQuery request, CancellationToken cancellationToken)
    {
        var noteId = request.NoteId;
        if(!await IsNoteContains(request.NoteId, cancellationToken))
        {
            return Result.Failure<GetIncomingRelationsResponse>(NoteErrors.NotFound);
        }

        IEnumerable<RelationResponse> rawRelations;
        try
        {
            rawRelations = await _relationsService.GetIncomingRelations(noteId, cancellationToken);
        }
        catch (Exception ex) 
        {
            throw new ServiceRequestException("Exception has thrown from relations service", ex);
        }
        if (!rawRelations.Any())
        {
            return new GetIncomingRelationsResponse(new List<RelationItem>());
        }

        var relations = await MapRawDataToRelations(noteId, rawRelations);

        return new GetIncomingRelationsResponse(relations);
    }

    private async Task<bool> IsNoteContains(Guid incomingNoteId, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        return (await connection.QueryAsync("SELECT * FROM notes WHERE id=@Id", new {Id = incomingNoteId})).Any();
    }

    private async Task<IEnumerable<RelationItem>> MapRawDataToRelations(Guid incomingNoteId, IEnumerable<RelationResponse> rawRelations)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var rawIncomingNote = await connection.QueryFirstAsync(
            "SELECT title, id FROM notes WHERE id=@Id", 
            new {Id=incomingNoteId});
        var incomingNote = new NoteItemDto(rawIncomingNote.title, rawIncomingNote.id);

        var rawOutgoingNotes = await connection.QueryAsync(
            "SELECT title, id FROM notes WHERE id=ANY(@Ids)", 
            new {Ids = rawRelations.Select(r=> r.OutgoingNoteId).ToArray()});
        var rawRelationsDictionary = rawRelations.ToDictionary(r => r.OutgoingNoteId);

        var relations = new List<RelationItem>();

        foreach(var rawOutgoingNote in rawOutgoingNotes)
        {
            var outgoingNote = new NoteItemDto(
                rawOutgoingNote.title,
                rawOutgoingNote.id);
            var relationId = rawRelationsDictionary[outgoingNote.Id].Id;

            relations.Add(new RelationItem(
                relationId,
                outgoingNote,
                incomingNote));
        }

        return relations;
    }
}
