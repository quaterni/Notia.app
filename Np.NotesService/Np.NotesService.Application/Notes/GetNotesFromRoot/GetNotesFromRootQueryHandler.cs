using System.Data;
using Dapper;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Dtos;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Application.Relations;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.GetNotesFromRoot;

public class GetNotesFromRootQueryHandler : IQueryHandler<GetNotesFromRootQuery, GetNotesFromRootResponse>
{
    private readonly IRelationsService _relationsService;

    public GetNotesFromRootQueryHandler(IRelationsService relationsService)
    {
        _relationsService = relationsService;
    }

    public async Task<Result<GetNotesFromRootResponse>> Handle(GetNotesFromRootQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var notes = await _relationsService.GetNotesFromRoot(
                request.UserId ?? throw new ApplicationException("User id was null"),
                cancellationToken);
            return new GetNotesFromRootResponse(notes);
        }
        catch (Exception e)
        {
            throw new ServiceRequestException("Exception has thrown from relations service", e);
        }
    }
}
