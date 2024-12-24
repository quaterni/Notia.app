
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Np.NotesService.Application.Abstractions.Outbox;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Infrastructure.Messaging.Abstractions;
using Np.NotesService.Infrastructure.Messaging.RabbitMq;
using RabbitMQ.Client;
using System.Text;


namespace Np.NotesService.Infrastructure.Outbox;

internal partial class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly IProducer<EventDto> _producer;
    private IModel? _channel;

    private readonly OutboxOptions _outboxOptions;

    public OutboxWorker(
        IServiceProvider provider, 
        IRabbitMqChannelFactory rabbitMqChannelFactory, 
        IOptions<OutboxOptions> options,
        ILogger<OutboxWorker> logger,
        IProducer<EventDto> producer)
    {
        _provider = provider;
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _logger = logger;
        _producer = producer;
        _outboxOptions = options.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxEntries(stoppingToken);
            await Task.Delay(TimeSpan.FromMilliseconds(_outboxOptions.CheckDelayMilliseconds));
        }
    }

    private async Task ProcessOutboxEntries(CancellationToken stoppingToken)
    {
        LogCheckEntries(_logger);
        using var scope = _provider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var outboxEntries = await outboxRepository.GetEntriesOrderedByRefreshTime(_outboxOptions.EntryLimitPerCheck);
        var entriesAmount = outboxEntries.Count();

        if (entriesAmount > 0) 
        {
            LogEntriesAmount(_logger, entriesAmount);
        }
        else
        {
            LogEmptyEntries(_logger);
        }

        foreach (var outboxEntry in outboxEntries)
        {
            LogProcessingEntry(_logger, outboxEntry.Id);
            await ProcessOutboxEntry(outboxRepository, unitOfWork, outboxEntry, stoppingToken);
        }
    }

    private async Task ProcessOutboxEntry(
        OutboxRepository outboxRepository, 
        IUnitOfWork unitOfWork, 
        OutboxEntry outboxEntry, 
        CancellationToken stoppingToken)
    {
        outboxEntry.RefreshTime = outboxEntry.RefreshTime
            .AddMinutes(_outboxOptions.EntryRefreshTimeMinutes);
        outboxRepository.Update(outboxEntry);
        await unitOfWork.SaveChangesAsync(stoppingToken);
        LogEntryRefreshed(_logger, outboxEntry.Id);

        var eventDto = new EventDto
        {
            EventName = outboxEntry.EventName,
            Body = outboxEntry.Data
        };
        SendEvent(eventDto);
        LogEventSent(_logger, eventDto.EventName);

        outboxRepository.Remove(outboxEntry);
        await unitOfWork.SaveChangesAsync(stoppingToken);
        LogRemoveEntry(_logger, outboxEntry.Id);
    }

    private void SendEvent(EventDto eventDto)
    {
        var body = JsonConvert.SerializeObject(eventDto);

        _channel.BasicPublish("events", string.Empty, body: Encoding.UTF8.GetBytes(body));

        // TODO: Refactor producers
        //_producer.SendAsync(eventDto).Wait();
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        LogStartingWorker(_logger);

        _channel = _rabbitMqChannelFactory.CreateChannel();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        LogStoppingWorker(_logger);

        _channel!.Dispose();
        _producer?.Dispose();
        return base.StopAsync(cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker starting")]
    static partial void LogStartingWorker(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker stopping")]
    static partial void LogStoppingWorker(ILogger logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Checking outbox entries")]
    static partial void LogCheckEntries(ILogger logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Outbox entries are empty")]
    static partial void LogEmptyEntries(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker take {Amount} entries")]
    static partial void LogEntriesAmount(ILogger logger, int amount);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker processing entry: {Id}")]
    static partial void LogProcessingEntry(ILogger logger, Guid id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker refreshed entry: {Id}")]
    static partial void LogEntryRefreshed(ILogger logger, Guid id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker sent event: {EventName}")]
    static partial void LogEventSent(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker remove entry: {Id}")]
    static partial void LogRemoveEntry(ILogger logger, Guid id);
}
