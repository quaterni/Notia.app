using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;

namespace Np.RelationsService.Application.Notes.AddNote
{
    internal class AddNoteCommandHandler : ICommandHandler<AddNoteCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IRootEntriesRepository _rootEntriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddNoteCommandHandler(
            INotesRepository notesRepository,
            IRootEntriesRepository rootEntriesRepository,
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _rootEntriesRepository = rootEntriesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            var noteId = request.NoteId;

            if(await _notesRepository.Contains(noteId))
            {
                return Result.Failed(Error.Undefined);
            }

            var result = Note.Create(noteId);

            if (result.IsFailed)
            {
                throw new Exception("Unexpected Exception");
            }

            var note = result.Value;
            _notesRepository.Add(note);

            var rootEntry = RootEntry.Create(note).Value;
            _rootEntriesRepository.Add(rootEntry);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
