
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Infrastructure.Messaging.RabbitMq;
using RabbitMQ.Client;
using System.Text;


namespace Np.NotesService.Infrastructure.Outbox;

internal class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IRabbitMqChannelFactory _rabbitMqChannelFactory;
    private IModel? _channel;

    private readonly OutboxOptions _outboxOptions;

    public OutboxWorker(
        IServiceProvider provider, 
        IRabbitMqChannelFactory rabbitMqChannelFactory, 
        IOptions<OutboxOptions> options)
    {
        _provider = provider;
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _outboxOptions = options.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var outboxEntries = await outboxRepository.GetEntriesOrderedByRefreshTime(_outboxOptions.EntryLimitPerCheck);

            foreach (var outboxEntry in outboxEntries)
            {
                outboxEntry.RefreshTime = outboxEntry.RefreshTime
                    .AddMinutes(_outboxOptions.EntryRefreshTimeMinutes);
                outboxRepository.Update(outboxEntry);
                await unitOfWork.SaveChangesAsync(stoppingToken);
                var eventDto = new EventDto 
                { 
                    EventName = outboxEntry.EventName, 
                    Body = outboxEntry.Data 
                };
                SendEvent(eventDto);

                outboxRepository.Remove(outboxEntry);
                await unitOfWork.SaveChangesAsync(stoppingToken);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(_outboxOptions.CheckDelayMilliseconds));
        }
    }

    private void SendEvent(EventDto eventDto)
    {
        var body = JsonConvert.SerializeObject(eventDto);

        _channel.BasicPublish("events", string.Empty, body: Encoding.UTF8.GetBytes(body));
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMqChannelFactory.CreateChannel();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel!.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
