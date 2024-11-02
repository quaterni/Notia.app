using Newtonsoft.Json;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;
using Np.UsersService.Core.Messaging.Outbox.Models;

namespace Np.UsersService.Core.Messaging.Outbox;

public partial class OutboxModelEventService : IModelEventService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OutboxModelEventService> _logger;

    public OutboxModelEventService(
        ApplicationDbContext dbContext,
        ILogger<OutboxModelEventService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void PublishEvent(IModelEvent modelEvent)
    {
        var data = JsonConvert.SerializeObject(modelEvent);
        var outboxEntry = new OutboxEntry()
        {
            Id = Guid.NewGuid(),
            Name = modelEvent.GetType().Name,
            Data = data,
            Created = DateTime.UtcNow,
            RefreshTime = DateTime.UtcNow
        };

        _dbContext.Add(outboxEntry);
        LogEventAdded(_logger, outboxEntry.Name);
    }

    [LoggerMessage(LogLevel.Information, Message ="Outbox event service add entry with event: {EventName}")]
    private static partial void LogEventAdded(ILogger logger, string eventName);
}
