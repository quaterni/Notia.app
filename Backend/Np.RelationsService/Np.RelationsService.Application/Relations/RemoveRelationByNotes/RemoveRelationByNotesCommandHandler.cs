
using Dapper;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;

namespace Np.RelationsService.Application.Relations.RemoveRelationByNotes;

internal class RemoveRelationByNotesCommandHandler : ICommandHandler<RemoveRelationByNotesCommand>
{
    private readonly IRelationsRepository _relationsRepository;
    private readonly INotesRepository _notesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRelationByNotesCommandHandler(
        IRelationsRepository relationsRepository,
        INotesRepository notesRepository, 
        IUnitOfWork unitOfWork)
    {
        _relationsRepository = relationsRepository;
        _notesRepository = notesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveRelationByNotesCommand request, CancellationToken cancellationToken)
    {
        var relation = await _relationsRepository.GetRelationByNotes(request.IncomingNote, request.OutgoingNote, cancellationToken);
        if(relation is null || !relation.Incoming.UserId.Equals(request.UserId))
        {
            return Result.Success();
        }
        relation.AddDomainEvent(new RelationRemovedEvent(relation.Id, relation.Outgoing.Id, relation.Incoming.Id));
        _relationsRepository.RemoveRelation(relation);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

}
