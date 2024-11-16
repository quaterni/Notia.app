
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Application.Relations;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.GetOutgoingNotes;

internal class GetOutgoingNotesQueryHandler : IQueryHandler<GetOutgoingNotesQuery, GetOutgoingNotesResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly IRelationsService _relationsService;

    public GetOutgoingNotesQueryHandler( 
        INotesRepository notesRepository,
        IRelationsService relationsService)
    {
        _notesRepository = notesRepository;
        _relationsService = relationsService;
    }

    public async Task<Result<GetOutgoingNotesResponse>> Handle(GetOutgoingNotesQuery request, CancellationToken cancellationToken)
    {
        var noteId = request.NoteId;
        var note = await _notesRepository.GetNoteById(noteId);
        if (note == null || !note.User.Id.Equals(request.UserId))
        {
            return Result.Failure<GetOutgoingNotesResponse>(GetOutgoingNotesErrors.NotFound);
        }
        try
        {
            var notes = await _relationsService.GetOutgoingNotes(noteId, cancellationToken);
            return new GetOutgoingNotesResponse(notes);
        }
        catch (Exception ex)
        {
            throw new ServiceRequestException("Exception has thrown from relations service", ex);
        }
    }
}
