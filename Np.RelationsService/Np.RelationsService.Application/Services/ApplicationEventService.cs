
using MediatR;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Notes.ApplicationEvents.NoteRemoved;
using Np.RelationsService.Application.Notes.Events.NoteAdded;
using Np.RelationsService.Application.Relations.ApplicationEvents.RelationCreated;
using Np.RelationsService.Application.Relations.ApplicationEvents.RelationRemoved;

namespace Np.RelationsService.Application.Services;

public class ApplicationEventService : IEventProcessor
{
    private readonly IPublisher _publisher;

    public ApplicationEventService(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Process(MessageBusEvent eventMessage)
    {
        switch (eventMessage.EventName)
        {
            case "NoteCreatedEvent":
                await _publisher.Publish(SerializeEvent<NoteCreatedApplicationEvent>(eventMessage.Body));
                break;
            case "NoteRemovedEvent":
                await _publisher.Publish(SerializeEvent<NoteRemovedApplicatonEvent>(eventMessage.Body));
                break;
            case "RelationCreatedEvent":
                await _publisher.Publish(SerializeEvent<RelationCreatedApplicationEvent>(eventMessage.Body));
                break;
            case "RelationRemovedEvent":
                await _publisher.Publish(SerializeEvent<RelationRemovedApplicationEvent>(eventMessage.Body));
                break;
        }
    }

    private T SerializeEvent<T>(string messageBusBoddy) where T:IApplicationEvent
    {
        var applicationEvent = JsonConvert.DeserializeObject<T>(messageBusBoddy);
        if (applicationEvent is null)
        {
            throw new ApplicationException("Can't serialize application event");
        }
        return applicationEvent;
    }

}
