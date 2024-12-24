
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Np.NotesService.Infrastructure.Messaging.Abstractions;
using Np.NotesService.Infrastructure.Messaging.Kafka.Options;
using Np.NotesService.Infrastructure.Messaging.Kafka.Serilizers;

namespace Np.NotesService.Infrastructure.Messaging.Kafka;

internal partial class KafkaNotesProducer<TMessage, TProducerOptions> : IProducer<TMessage>
    where TProducerOptions :  class, IProducerOptions
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly ILogger<KafkaNotesProducer<TMessage, TProducerOptions>> _logger;
    private readonly string _topic;

    public KafkaNotesProducer(
        ILogger<KafkaNotesProducer<TMessage, TProducerOptions>> logger,
        IOptions<KafkaConnection> kafkaConnectionOptions,
        IOptions<TProducerOptions> options)
    {
        var producerOptions = options.Value;
        var kafkaConnection = kafkaConnectionOptions.Value;
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConnection.Servers
        };

        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new JsonSerializer<TMessage>())
            .SetLogHandler((p, logMessage)=>
                LogMessage(
                    logger,
                    p.GetType().Name,
                    logMessage.Facility, 
                    logMessage.Level, 
                    logMessage.Message))
            .SetErrorHandler((p, error)=>
                LogError(
                    logger, 
                    p.GetType().Name, 
                    error.Code, 
                    error.IsFatal, 
                    error.Reason))
            .Build();
        _logger = logger;

        _topic = producerOptions.Topic;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }

    public async Task SendAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await _producer.ProduceAsync(_topic, new Message<string, TMessage>()
        {
            Key = "",
            Value = message
        },cancellationToken);
    }

    [LoggerMessage(
        Level =LogLevel.Information, 
        Message ="{ProducerName}: {Facility}-{Level} Message: {Message}")]
    private static partial  void LogMessage(
        ILogger logger,
        string producerName,
        string facility, 
        SyslogLevel level, 
        string message);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "{ProducerName}: Error {Code} (is fatal:{IsFatal}) Message: {Message}")]
    private static partial void LogError(
        ILogger logger,
        string producerName,
        ErrorCode code,
        bool isFatal,
        string message);
}
