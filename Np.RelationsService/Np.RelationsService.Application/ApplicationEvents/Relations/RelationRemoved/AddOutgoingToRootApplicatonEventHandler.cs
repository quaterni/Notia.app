
using MediatR;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.RootEntries.AddRootEntry;
using Np.RelationsService.Domain.Relations;

namespace Np.RelationsService.Application.ApplicationEvents.Relations.RelationRemoved;

internal class AddOutgoingToRootApplicatonEventHandler : IApplicationEventHandler<RelationRemovedApplicationEvent>
{
    private readonly ISender _sender;
    private readonly IRelationsRepository _relationsRepository;

    public AddOutgoingToRootApplicatonEventHandler(
        ISender sender,
        IRelationsRepository relationsRepository)
    {
        _sender = sender;
        _relationsRepository = relationsRepository;
    }

    public async Task Handle(RelationRemovedApplicationEvent notification, CancellationToken cancellationToken)
    {
        var outgoingNoteId = notification.OutgoingNoteId;
        if (await _relationsRepository.HasOutgoingRelations(outgoingNoteId))
        {
            return;
        }

        await _sender.Send(new AddRootEntryCommand(outgoingNoteId));
    }
}
