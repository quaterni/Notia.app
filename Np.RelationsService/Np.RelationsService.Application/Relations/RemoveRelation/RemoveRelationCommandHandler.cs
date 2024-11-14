using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;

namespace Np.RelationsService.Application.Relations.RemoveRelation
{
    internal class RemoveRelationCommandHandler : ICommandHandler<RemoveRelationCommand>
    {
        private readonly IRelationsRepository _relationsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRelationCommandHandler(
            IRelationsRepository relationsRepository,
            IUnitOfWork unitOfWork)
        {
            _relationsRepository = relationsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveRelationCommand request, CancellationToken cancellationToken)
        {
            var relation = await _relationsRepository.GetRelationById(request.RelationId);

            if (relation == null) 
            {
                return Result.Failed(RemoveRelationErrors.NotFound);
            }

            if (!relation.Incoming.UserId.Equals(request.UserId))
            {
                return Result.Failed(RemoveRelationErrors.NotFound);
            }

            _relationsRepository.RemoveRelation(relation);

            relation.AddDomainEvent(new RelationRemovedEvent(relation.Id, relation.Outgoing.Id, relation.Incoming.Id));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
