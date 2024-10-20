
using MediatR;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Notes.RemoveNote;

namespace Np.RelationsService.Application.Notes.ApplicationEvents.NoteRemoved;

internal class RemoveNoteApplicationEventHandler : IApplicationEventHandler<NoteRemovedApplicatonEvent>
{
    private readonly ISender _sender;

    public RemoveNoteApplicationEventHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(NoteRemovedApplicatonEvent notification, CancellationToken cancellationToken)
    {
        await _sender.Send(new RemoveNoteCommand(notification.NoteId));
    }
}
