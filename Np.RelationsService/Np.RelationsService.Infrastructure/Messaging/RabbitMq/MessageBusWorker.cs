
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq;

internal partial class MessageBusWorker : BackgroundService
{
    [LoggerMessage(Level = LogLevel.Information, Message ="Message bus worker consuming queue: {QueueName}")]
    private static partial void LogConsumingQueue(ILogger logger, string queueName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Message bus worker recived message: {DeliveryTag}")]
    private static partial void LogReceivedMessage(ILogger logger, ulong deliveryTag);

    [LoggerMessage(Level = LogLevel.Error, Message = "Message bus worker failed serialize message: {DeliveryTag}")]
    private static partial void LogFailedSerializeMessage(ILogger logger, ulong deliveryTag);

    [LoggerMessage(Level = LogLevel.Information, Message = "Message bus worker acked message: {DeliveryTag}")]
    private static partial void LogAckedMessage(ILogger logger, ulong deliveryTag);

    private readonly RabbitMqChannelFactory _rabbitMqChannelFactory;
    private readonly QueueOptions _queueOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageBusWorker> _logger;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public MessageBusWorker(
        RabbitMqChannelFactory rabbitMqChannelFactory,
        IServiceProvider serviceProvider,
        ILogger<MessageBusWorker> logger,
        IOptions<QueueOptions> options)
    {
        _queueOptions = options.Value;
        _rabbitMqChannelFactory = rabbitMqChannelFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = _rabbitMqChannelFactory.Create();

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += Consumer_Received;

        _channel.BasicConsume(
            _queueOptions.Name,
            autoAck: false,
            _consumer);

        LogConsumingQueue(_logger, _queueOptions.Name);

        return base.StartAsync(cancellationToken);
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        LogReceivedMessage(_logger, e.DeliveryTag);

        var jsonString = Encoding.UTF8.GetString(e.Body.ToArray());
        var eventMessage = JsonConvert.DeserializeObject<MessageBusEvent>(jsonString);

        if (eventMessage == null)
        {
            LogFailedSerializeMessage(_logger, e.DeliveryTag);
            throw new ApplicationException("Can't serialize event message.");
        }

        using var scope = _serviceProvider.CreateScope();
        var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
        await eventProcessor.Process(eventMessage);

        _channel!.BasicAck(e.DeliveryTag, false);
        LogAckedMessage(_logger, e.DeliveryTag);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel!.Dispose();
        _consumer!.Received -= Consumer_Received;        
        return base.StopAsync(cancellationToken);
    }
}
