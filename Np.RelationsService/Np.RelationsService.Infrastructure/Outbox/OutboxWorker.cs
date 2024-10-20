
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq;
using RabbitMQ.Client;
using System.Text;

namespace Np.RelationsService.Infrastructure.Outbox;

internal partial class OutboxWorker : BackgroundService
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Outbox worker started")]
    private static partial void LogStart(ILogger logger);

    [LoggerMessage(Level =LogLevel.Trace, Message ="Outboxes are empty")]
    private static partial void LogEmptyOutboxes(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker get {Count} outboxes")]
    private static partial void LogOutboxesCount(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker start processing outbox {OutboxId}")]
    private static partial void LogProcessingOutbox(ILogger logger, Guid outboxId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker updated refresh time {OutboxId}")]
    private static partial void LogUpdateRefreshTimeOutbox(ILogger logger, Guid outboxId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker sent event: {EventName}")]
    private static partial void LogSentEvent(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Outbox worker removed outbox: {OutboxId}")]
    private static partial void LogRemovedOutbox(ILogger logger, Guid outboxId);

    private readonly RabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxWorker> _logger;
    private IModel? _channel;

    public OutboxWorker(
        RabbitMqChannelFactory rabbitMqChannelFactory,
        IServiceProvider serviceProvider,
        ILogger<OutboxWorker> logger)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var outboxEntries = await outboxRepository.GetOrderedByRefreshTime(100);
            var outboxCount = outboxEntries.Count();

            if(outboxCount == 0)
            {
                LogEmptyOutboxes(_logger);
            }
            else
            {
                LogOutboxesCount(_logger, outboxCount);
            }

            foreach (var outboxEntry in outboxEntries) 
            {
                var outboxEntryId = outboxEntry.Id;
                LogProcessingOutbox(_logger, outboxEntryId);

                outboxEntry.RefreshTime = outboxEntry.RefreshTime.AddMinutes(10);
                outboxRepository.Update(outboxEntry);
                await unitOfWork.SaveChangesAsync(stoppingToken);
                LogUpdateRefreshTimeOutbox(_logger, outboxEntryId);

                SendMessage(new MessageBusEvent(outboxEntry.EventName, outboxEntry.Data));
                LogSentEvent(_logger, outboxEntry.EventName);

                outboxRepository.Remove(outboxEntry);
                await unitOfWork.SaveChangesAsync();
                LogRemovedOutbox(_logger, outboxEntryId);
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }


    private void SendMessage(MessageBusEvent eventMessage)
    {
        var json = JsonConvert.SerializeObject(eventMessage);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "events", 
            routingKey: string.Empty, 
            body: body);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMqChannelFactory.Create();

        LogStart(_logger);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
