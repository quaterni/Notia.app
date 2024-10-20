
using MediatR;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.UnitTests.RootEntires.RemoveRootEntry;
using Np.RelationsService.Domain.RootEntries;

namespace Np.RelationsService.Application.Relations.ApplicationEvents.RelationCreated;

internal class RemoveFromRootApplicatonEventHandler : IApplicationEventHandler<RelationCreatedApplicationEvent>
{
    private readonly ISender _sender;
    private readonly IRootEntriesRepository _rootEntriesRepository;

    public RemoveFromRootApplicatonEventHandler(ISender sender, IRootEntriesRepository rootEntriesRepository)
    {
        _sender = sender;
        _rootEntriesRepository = rootEntriesRepository;
    }

    public async Task Handle(RelationCreatedApplicationEvent notification, CancellationToken cancellationToken)
    {
        var rootEntry = await _rootEntriesRepository.GetRootEntryById(notification.OutgoingNoteId);

        if (rootEntry == null)
        {
            // TODO: log
            return;
        }

        await _sender.Send(new RemoveRootEntryCommand(rootEntry.Id));
    }
}
