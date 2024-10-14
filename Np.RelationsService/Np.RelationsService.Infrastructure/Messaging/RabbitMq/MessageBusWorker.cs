
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq;

internal class MessageBusWorker : BackgroundService
{
    private readonly RabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly IServiceProvider _serviceProvider;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public MessageBusWorker(
        RabbitMqChannelFactory rabbitMqChannelFactory,
        IServiceProvider serviceProvider)
    {
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMqChannelFactory.Create();

        var workingQueueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(
            workingQueueName,
            exchange: "events",
            routingKey: string.Empty,
            arguments: null);

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += Consumer_Received;

        _channel.BasicConsume(
            workingQueueName,
            autoAck: false,
            _consumer);

        return base.StartAsync(cancellationToken);
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        var jsonString = Encoding.UTF8.GetString(e.Body.ToArray());
        var eventMessage = JsonConvert.DeserializeObject<MessageBusEvent>(jsonString);

        if (eventMessage == null)
        {
            throw new ApplicationException("Can't serialize event message.");
        }

        using var scope = _serviceProvider.CreateScope();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
        await eventProcessor.Process(eventMessage);

        _channel!.BasicAck(e.DeliveryTag, false);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel!.Dispose();
        _consumer!.Received -= Consumer_Received;        
        return base.StopAsync(cancellationToken);
    }
}
