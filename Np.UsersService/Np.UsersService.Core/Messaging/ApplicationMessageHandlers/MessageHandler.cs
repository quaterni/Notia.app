using MediatR;
using Newtonsoft.Json;
using Np.UsersService.Core.Messaging.Models;

namespace Np.UsersService.Core.Messaging.ApplicationMessageHandlers;

public partial class MessageHandler
{
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(
        ILogger<MessageHandler> logger,
        IPublisher publisher)
    {
        _logger = logger;
    }

    public async Task Handle(MessageBusEvent message)
    {
        return;
    }

    [LoggerMessage(Level=LogLevel.Information, Message="Event deserialized: {EventName}")]
    private static partial void LogEventDeserialized(ILogger logger, string eventName);

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
