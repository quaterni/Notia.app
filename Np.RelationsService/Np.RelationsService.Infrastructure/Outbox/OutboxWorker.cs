
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq;
using RabbitMQ.Client;
using System.Text;

namespace Np.RelationsService.Infrastructure.Outbox;

internal class OutboxWorker : BackgroundService
{
    private readonly RabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly IServiceProvider _serviceProvider;
    private IModel? _channel;

    public OutboxWorker(
        RabbitMqChannelFactory rabbitMqChannelFactory,
        IServiceProvider serviceProvider)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _serviceProvider = serviceProvider;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            foreach (var outboxEntry in await outboxRepository.GetOrderedByRefreshTime(100)) 
            {
                outboxEntry.RefreshTime = outboxEntry.RefreshTime.AddMinutes(10);
                outboxRepository.Update(outboxEntry);
                await unitOfWork.SaveChangesAsync(stoppingToken);

                SendMessage(new MessageBusEvent(outboxEntry.EventName, outboxEntry.Data));

                outboxRepository.Remove(outboxEntry);
                await unitOfWork.SaveChangesAsync();

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

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
