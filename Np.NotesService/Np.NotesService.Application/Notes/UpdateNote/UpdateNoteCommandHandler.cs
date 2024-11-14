using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.UpdateNote
{
    internal class UpdateNoteCommandHandler : ICommandHandler<UpdateNoteCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateNoteCommandHandler(
            INotesRepository notesRepository, 
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider)
        {
            _notesRepository = notesRepository;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _notesRepository.GetNoteById(request.NoteId);

            if (note == null)
            { 
                return Result.Failure(UpdateNoteErrors.NotFound);
            }

            if (!note.User.Id.Equals(request.UserId!.Value))
            {
                return Result.Failure(UpdateNoteErrors.NotFound);
            }

            note.UpdateNote(request.Data, _dateTimeProvider);
            _notesRepository.Update(note);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch(InvalidOperationException ex)
            {
                throw new ConcurrencyException("ConcurrencyException was thrown when attempting to update note", ex);
            }
        }
    }
}
