using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Domain.Notes.Events;

namespace Np.NotesService.Application.Notes.RemoveNote
{
    internal class RemoveNoteCommandHandler : ICommandHandler<RemoveNoteCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveNoteCommandHandler(
            INotesRepository notesRepository, 
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
        {
            var note = await _notesRepository.GetNoteById(request.NoteId);
            if (note == null)
            {
                return Result.Success();
            }

            if (!note.User.Id.Equals(request.UserId!.Value))
            {
                return Result.Success();
            }
            _notesRepository.Delete(note);
            note.AddDomainEvent(new NoteRemovedEvent(note.Id));

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
