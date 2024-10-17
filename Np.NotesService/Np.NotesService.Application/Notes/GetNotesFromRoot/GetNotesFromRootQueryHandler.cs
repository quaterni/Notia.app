using System.Data;
using Dapper;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Shared;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Application.Relations.Service;

namespace Np.NotesService.Application.Notes.GetNotesFromRoot;

public class GetNotesFromRootQueryHandler : IQueryHandler<GetNotesFromRootQuery, GetNotesFromRootResponse>
{
    private readonly IRelationsService _relationsService;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetNotesFromRootQueryHandler(
        IRelationsService relationsService,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _relationsService = relationsService;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetNotesFromRootResponse>> Handle(GetNotesFromRootQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<NoteResponse> relationsResponse;
        try
        {
            relationsResponse = await _relationsService.GetNotesFromRoot(cancellationToken);
        }
        catch(Exception e)
        {
            throw new ServiceRequestException("Exception has thrown from relations service", e);
        }

        if(!relationsResponse.Any())
        {
            return new GetNotesFromRootResponse(new List<NoteItem>());
        }

        var ids = relationsResponse.Select(r=> r.NoteId).ToArray();
        using var connection = _sqlConnectionFactory.CreateConnection();
        var dbResponse = await connection.QueryAsync("SELECT title, id FROM notes WHERE id =ANY(@Ids)", new {Ids = ids.ToArray()});
        var noteResponses = dbResponse.Select(r=> new NoteItem(r.title, r.id));

        return new GetNotesFromRootResponse(noteResponses);
    }
}
