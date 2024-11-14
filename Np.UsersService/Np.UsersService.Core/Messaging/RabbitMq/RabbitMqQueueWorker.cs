
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Np.UsersService.Core.Messaging.MessageHandling;
using Np.UsersService.Core.Messaging.Models;
using Np.UsersService.Core.Messaging.RabbitMq.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;

namespace Np.UsersService.Core.Messaging.RabbitMq;

public partial class RabbitMqQueueWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqQueueWorker> _logger;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;

    public RabbitMqQueueWorker(IServiceProvider serviceProvider, ILogger<RabbitMqQueueWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var queueOptions = _serviceProvider.GetRequiredService<IOptions<RabbitMqQueueOptions>>().Value;
        var channelFactory = _serviceProvider.GetRequiredService<RabbitMqChannelFactory>();

        _channel = channelFactory.CreateChannel();

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += Consumer_Received;

        _channel.BasicConsume(
            queueOptions.QueueName,
            autoAck: false,
            _consumer);

        LogWorkerStarted(_logger);

        return base.StartAsync(cancellationToken);
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _consumer!.Received -= Consumer_Received;
        LogWorkerStopped(_logger);
        return base.StopAsync(cancellationToken);
    }

    [LoggerMessage(Level= LogLevel.Information, Message= "RabbitMqQueueWorker started")]
    private static partial void LogWorkerStarted(ILogger logger);


    [LoggerMessage(Level= LogLevel.Information, Message= "RabbitMqQueueWorker stopped")]
    private static partial void LogWorkerStopped(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMqQueueWorker receive message: {DeliveryTag}")]
    private static partial void LogMessageReceived(ILogger logger, ulong deliveryTag);

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMqQueueWorker acked message: {DeliveryTag}")]
    private static partial void LogAckedMessage(ILogger logger, ulong deliveryTag);

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
    {
        LogMessageReceived(_logger, e.DeliveryTag);

        using var scope = _serviceProvider.CreateScope();
        var messageHandler = scope.ServiceProvider.GetRequiredService<MessageHandler>();
        var message = DeserializeMessage(e.Body.ToArray());

        await messageHandler.Handle(message);

        _channel!.BasicAck(e.DeliveryTag, false);
        LogAckedMessage(_logger, e.DeliveryTag);
    }

    private MessageBusEvent DeserializeMessage(byte[] body)
    {
        var message = JsonConvert.DeserializeObject<MessageBusEvent>(Encoding.UTF8.GetString(body));

        if (message == null)
        {
            throw new ApplicationException("Cannot serialize message from message bus");
        }

        return message;
    }
}
