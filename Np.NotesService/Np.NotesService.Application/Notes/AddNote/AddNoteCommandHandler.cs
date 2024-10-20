using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.AddNote
{
    internal class AddNoteCommandHandler : ICommandHandler<AddNoteCommand, Guid>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AddNoteCommandHandler(
            INotesRepository notesRepository, 
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider)
        {
            _notesRepository = notesRepository;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Result<Guid>> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            var note = Note.Create(request.Data, _dateTimeProvider);

            _notesRepository.Add(note);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return note.Id;
        }
    }
}
