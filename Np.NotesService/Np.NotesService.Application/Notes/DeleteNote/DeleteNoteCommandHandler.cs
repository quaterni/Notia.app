using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.DeleteNote
{
    internal class DeleteNoteCommandHandler : ICommandHandler<DeleteNoteCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNoteCommandHandler(
            INotesRepository notesRepository, 
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _notesRepository.GetNoteById(request.NoteId);

            if (note == null) 
            {
                return Result.Success();
            }

            _notesRepository.Delete(note);
            try
            {
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                throw new ConcurrencyException("ConcurrencyException was thrown when attempting to delete note", ex);
            }
        }
    }
}
