using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;

namespace Np.RelationsService.Application.Relations.AddRelation
{
    internal class AddRelationCommandHandler : ICommandHandler<AddRelationCommand>
    {
        private readonly INotesRepository _notesRepository;
        private readonly IRelationsRepository _relationsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddRelationCommandHandler(
            INotesRepository notesRepository, 
            IRelationsRepository relationsRepository,
            IUnitOfWork unitOfWork)
        {
            _notesRepository = notesRepository;
            _relationsRepository = relationsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddRelationCommand request, CancellationToken cancellationToken)
        {
            if (request.IncomingNoteId.Equals(request.OutgoingNoteId))
            {
                return Result.Failed(RelationErrors.ItselfRelation);
            }

            var incomingNote = await _notesRepository.GetNoteById(request.IncomingNoteId);
            var outgoingNote = await _notesRepository.GetNoteById(request.OutgoingNoteId);

            if(incomingNote == null || outgoingNote == null)
            {
                return Result.Failed(NotesErrors.NotFound);
            }

            if(!incomingNote.UserId.Equals(request.UserId) || !outgoingNote.UserId.Equals(request.UserId))
            {
                return Result.Failed(NotesErrors.NotFound);
            }

            if(await _relationsRepository.HasRelation(incomingNote, outgoingNote))
            {
                return Result.Failed(RelationErrors.Duplication);
            }

            var relation = Relation.Create(outgoingNote, incomingNote).Value;

            relation.AddDomainEvent(new RelationCreatedEvent(
                relation.Id, 
                relation.Outgoing.Id,
                relation.Incoming.Id));

             _relationsRepository.AddRelation(relation);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
