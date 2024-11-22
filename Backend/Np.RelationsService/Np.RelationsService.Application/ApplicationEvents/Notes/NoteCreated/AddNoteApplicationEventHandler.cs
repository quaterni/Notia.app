
using MediatR;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Notes.AddNote;

namespace Np.RelationsService.Application.ApplicationEvents.Notes.NoteCreated;

internal class AddNoteApplicationEventHandler : IApplicationEventHandler<NoteCreatedApplicationEvent>
{
    private readonly ISender _sender;

    public AddNoteApplicationEventHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(NoteCreatedApplicationEvent notification, CancellationToken cancellationToken)
    {
        await _sender.Send(new AddNoteCommand(notification.NoteId, notification.UserId));
    }
}
