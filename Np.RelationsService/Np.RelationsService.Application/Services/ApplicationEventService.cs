
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Notes.ApplicationEvents.NoteRemoved;
using Np.RelationsService.Application.Notes.Events.NoteAdded;
using Np.RelationsService.Application.Relations.ApplicationEvents.RelationCreated;
using Np.RelationsService.Application.Relations.ApplicationEvents.RelationRemoved;

namespace Np.RelationsService.Application.Services;

public partial class ApplicationEventService : IEventProcessor
{
    [LoggerMessage(Level =LogLevel.Information, Message ="Published event: {EventName}")]
    private static partial void LogEventPublished(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Accepted unknown event: {EventName}")]
    private static partial void LogUnknownEvent(ILogger logger, string eventName);

    private readonly IPublisher _publisher;
    private readonly ILogger<ApplicationEventService> _logger;

    public ApplicationEventService(
        IPublisher publisher,
        ILogger<ApplicationEventService> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Process(MessageBusEvent eventMessage)
    {
        switch (eventMessage.EventName)
        {
            case "NoteCreatedEvent":
                LogEventPublished(_logger, eventMessage.EventName);
                await _publisher.Publish(SerializeEvent<NoteCreatedApplicationEvent>(eventMessage.Body));
                break;
            case "NoteRemovedEvent":
                LogEventPublished(_logger, eventMessage.EventName);
                await _publisher.Publish(SerializeEvent<NoteRemovedApplicatonEvent>(eventMessage.Body));
                break;
            case "RelationCreatedEvent":
                LogEventPublished(_logger, eventMessage.EventName);
                await _publisher.Publish(SerializeEvent<RelationCreatedApplicationEvent>(eventMessage.Body));
                break;
            case "RelationRemovedEvent":
                LogEventPublished(_logger, eventMessage.EventName);
                await _publisher.Publish(SerializeEvent<RelationRemovedApplicationEvent>(eventMessage.Body));
                break;
            default:
                LogUnknownEvent(_logger, eventMessage.EventName);
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
