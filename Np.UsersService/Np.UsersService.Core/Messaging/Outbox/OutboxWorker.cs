
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Messaging.Outbox.Models;
using Np.UsersService.Core.Messaging.Outbox.Options;
using Np.UsersService.Core.Messaging.RabbitMq;
using Np.UsersService.Core.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using System.Text;

namespace Np.UsersService.Core.Messaging.Outbox;

public partial class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<OutboxWorker> _logger;

    private readonly OutboxOptions _outboxOptions;

    private IModel? _channel;

    public OutboxWorker(
        IServiceProvider serviceProvider,
        ILogger<OutboxWorker> logger,
        IOptions<OutboxOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _outboxOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) 
        {
            using var scope = _serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var entries = await GetOutboxEntriesAsync(dbContext, _outboxOptions.CheckLimit, stoppingToken);
            var entriesCount = entries.Count();
            if (entriesCount > 0)
            {
                LogTakenEntriesCount(_logger, entriesCount);
            }

            foreach (var entry in entries)
            {
                LogStartProcessEntry(_logger, entry.Id);
            }
            await Task.Delay(TimeSpan.FromSeconds(_outboxOptions.CheckTimeoutInSeconds));
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var channelFactory = _serviceProvider.GetRequiredService<RabbitMqChannelFactory>();

        _channel = channelFactory.CreateChannel();

        LogStartWorker(_logger);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel!.Dispose();
        LogStopWorker(_logger);
        return base.StopAsync(cancellationToken);
    }

    [LoggerMessage(Level =LogLevel.Information, Message = "Outbox worker started")]
    private static partial void LogStartWorker(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker stopped")]
    private static partial void LogStopWorker(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker took {Count} entries")]
    private static partial void LogTakenEntriesCount(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker start process entry: {EntryId}")]
    private static partial void LogStartProcessEntry(ILogger logger, Guid entryId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker updated resfresh time, entry id: {EntryId}")]
    private static partial void LogUpdatedEntry(ILogger logger, Guid entryId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker published message: {Name}")]
    private static partial void LogPublishedMessage(ILogger logger, string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker removed entry: {EntryId}")]
    private static partial void LogRemovedEntry(ILogger logger, Guid entryId);

    private async Task<IEnumerable<OutboxEntry>> GetOutboxEntriesAsync(
        ApplicationDbContext dbContext, 
        int limit, 
        CancellationToken cancellationToken)
    {
        return await dbContext.Set<OutboxEntry>()
            .OrderBy(e=> e.RefreshTime)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    private async Task ProcessEntryAsync(
        OutboxEntry entry,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        entry.RefreshTime.AddMinutes(_outboxOptions.RefreshAdditionInMinutes);
        dbContext.Update(entry);
        await dbContext.SaveChangesAsync();
        LogUpdatedEntry(_logger, entry.Id);

        SendMessage(entry.Name, entry.Data, cancellationToken);

        dbContext.Remove(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
        LogRemovedEntry(_logger, entry.Id);
    }

    private void SendMessage(string name, string data, CancellationToken cancellationToken)
    {
        var body = JsonConvert.SerializeObject(new { Name = name, Body = data });
        var exchangeOptions = _serviceProvider.GetRequiredService<IOptions<RabbitMqExchangeOptions>>().Value;

        _channel.BasicPublish(
            exchangeOptions.ExchangeName, 
            string.Empty, 
            body: Encoding.UTF8.GetBytes(body));

        LogPublishedMessage(_logger, name);
    }
}
