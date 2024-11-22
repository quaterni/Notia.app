using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;
using Np.RelationsService.Domain.RootEntries;

namespace Np.RelationsService.Application.Notes.RemoveNote
{
    internal class RemoveNoteCommandHandler : ICommandHandler<RemoveNoteCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IRelationsRepository _relationsRepository;
        private readonly IRootEntriesRepository _rootEntriesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveNoteCommandHandler(
            INotesRepository notesRepository, 
            IRelationsRepository relationsRepository,
            IRootEntriesRepository rootEntriesRepository,
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _relationsRepository = relationsRepository;
            _rootEntriesRepository = rootEntriesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveNoteCommand request, CancellationToken cancellationToken)
        {
            var noteId = request.NoteId;

            var note = await _notesRepository.GetNoteById(noteId);

            if (note == null)
            {
                return Result.Failed(NotesErrors.NotFound);
            }

            _notesRepository.Remove(note);

            await RemoveRootEntry(noteId);

            await RemoveRelations(noteId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private async Task RemoveRootEntry(Guid noteId)
        {
            var entryRoot = await _rootEntriesRepository.GetRootEntryByNoteId(noteId);

            if (entryRoot != null)
            {
                _rootEntriesRepository.Remove(entryRoot);
            }
        }

        private async Task RemoveRelations(Guid noteId)
        {
            var incomingRelations = await _relationsRepository.GetIncomingRelations(noteId);
            foreach (var incomingRelation in incomingRelations)
            {
                _relationsRepository.RemoveRelation(incomingRelation);
                incomingRelation.AddDomainEvent(
                    new RelationRemovedEvent(
                        incomingRelation.Id, 
                        incomingRelation.Outgoing.Id, 
                        incomingRelation.Incoming.Id));
            }

            var outgiongRelations = await _relationsRepository.GetOutgoingRelations(noteId);
            foreach (var outgoingRelation in outgiongRelations)
            {
                _relationsRepository.RemoveRelation(outgoingRelation);
                outgoingRelation.AddDomainEvent(
                    new RelationRemovedEvent(
                        outgoingRelation.Id,
                        outgoingRelation.Outgoing.Id,
                        outgoingRelation.Incoming.Id));
            }
        }
    }
}
