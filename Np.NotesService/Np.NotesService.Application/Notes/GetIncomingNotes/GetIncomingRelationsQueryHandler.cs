
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Relations;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.GetIncomingNotes;

internal class GetIncomingRelationsQueryHandler : IQueryHandler<GetIncomingRelationsQuery, GetIncomingNotesResponse>
{
    private readonly INotesRepository _notesRepository;
    private readonly IRelationsService _relationsService;

    public GetIncomingRelationsQueryHandler(
        INotesRepository notesRepository,
        IRelationsService relationsService)
    {
        _notesRepository = notesRepository;
        _relationsService = relationsService;
    }

    public async Task<Result<GetIncomingNotesResponse>> Handle(GetIncomingRelationsQuery request, CancellationToken cancellationToken)
    {
        var noteId = request.NoteId;
        var note = await _notesRepository.GetNoteById(noteId);
        if (note == null || !note.User.Id.Equals(request.UserId))
        {
            return Result.Failure<GetIncomingNotesResponse>(GetIncomingNotesErrors.NotFound);
        }
        try
        {
            var notes = await _relationsService.GetIncomingNotes(noteId, cancellationToken);
            return new GetIncomingNotesResponse(notes);
        }
        catch (Exception ex)
        {
            throw new ServiceRequestException("Exception has thrown from relations service", ex);
        }
    }
}
