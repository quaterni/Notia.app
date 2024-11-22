using MediatR;
using Newtonsoft.Json;
using Np.UsersService.Core.Messaging.MessageHandling.Users.UserCreatedSecure;
using Np.UsersService.Core.Messaging.MessageHandling.Users.UserDeleted;
using Np.UsersService.Core.Messaging.MessageHandling.Users.UserUpdatedSecure;
using Np.UsersService.Core.Messaging.ModelEvents.Users;
using Np.UsersService.Core.Messaging.Models;

namespace Np.UsersService.Core.Messaging.MessageHandling;

public partial class MessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly IPublisher _publisher;

    public MessageHandler(
        ILogger<MessageHandler> logger,
        IPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(MessageBusEvent message)
    {
        switch (message.EventName)
        {
            case nameof(UserCreatedSecureEvent):
                {
                    var e = DeserializeEvent<UserCreatedSecureApplicationEvent>(message.Body);
                    LogPublishEvent(_logger, e.GetType().Name); ;
                    await _publisher.Publish(e);
                    break;
                }
            case nameof(UserUpdatedSecureEvent):
                {
                    var e = DeserializeEvent<UserUpdatedSecureApplicationEvent>(message.Body);
                    LogPublishEvent(_logger, e.GetType().Name); ;
                    await _publisher.Publish(e);
                    break;
                }
            case nameof(UserDeletedSecureEvent):
                {
                    var e = DeserializeEvent<UserDeletedSecureApplicationEvent>(message.Body);
                    LogPublishEvent(_logger, e.GetType().Name); ;
                    await _publisher.Publish(e);
                    break;
                }
            default:
                LogUnknownEvent(_logger, message.EventName);
                break;
        }
        return;
    }

    [LoggerMessage(Level=LogLevel.Information, Message="Event deserialized: {EventName}")]
    private static partial void LogEventDeserialized(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Message handler publish event: {EventName}")]
    private static partial void LogPublishEvent(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Message handler doesn't have handler to event: {EventName}")]
    private static partial void LogUnknownEvent(ILogger logger, string eventName);

    private TEvent DeserializeEvent<TEvent>(string data) where TEvent : IApplicationEvent
    {
        var e = JsonConvert.DeserializeObject<TEvent>(data);
        if(e is null)
        {
            throw new ApplicationException("Message bus event cannot serialize.");
        }

        LogEventDeserialized(_logger, e.GetType().Name);
        return e;
    }
}
