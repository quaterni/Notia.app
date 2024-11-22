using Microsoft.Extensions.Options;
using Np.UsersService.Core.Messaging.RabbitMq.Options;
using RabbitMQ.Client;

namespace Np.UsersService.Core.Messaging.RabbitMq;

public partial class RabbitMqChannelFactory: IDisposable
{
    private readonly IConnection _connection;

    private readonly RabbitMqExchangeOptions _exchangeOptions;

    private readonly ILogger<RabbitMqChannelFactory> _logger;

    private bool _disposed;

    public RabbitMqChannelFactory(
        ILogger<RabbitMqChannelFactory> logger,
        IOptions<RabbitMqConnecitonOptions> options,
        IOptions<RabbitMqExchangeOptions> exchangeOptions)
    {
        var connectionOptions = options.Value;
        _exchangeOptions = exchangeOptions.Value;

        _connection = new ConnectionFactory()
        {
            HostName = connectionOptions.Host,
            UserName = connectionOptions.Username,
            Password = connectionOptions.Password,
            Port = connectionOptions.Port,
        }.CreateConnection();
        _logger = logger;

        _connection.ConnectionShutdown += Connection_ConnectionShutdown;

        LogConnectionCreated(_logger);
    }


    public IModel CreateChannel()
    {
        var model = _connection.CreateModel();

        model.ExchangeDeclare(
            _exchangeOptions.ExchangeName,
            _exchangeOptions.Type,
            _exchangeOptions.Durable,
            _exchangeOptions.AutoDelete,
            null);

        LogChannelCreated(_logger, model.ChannelNumber);

        return model;
    }

    public void Dispose()
    {
        if(_disposed)
            return;
        _connection.Dispose();
        LogConnectionDisposed(_logger);
        _disposed = true;
    }

    [LoggerMessage(Level = LogLevel.Information, Message ="RabbitMQ connection created")]
    private static partial void LogConnectionCreated(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ connection disposed")]
    private static partial void LogConnectionDisposed(ILogger logger);


    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ channel created {ChannelNumber}")]
    private static partial void LogChannelCreated(ILogger logger, int channelNumber);

    [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ connection was unexpectedly shutdown")]
    private static partial void LogConnectionShutdown(ILogger logger);

    private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        LogConnectionShutdown(_logger);
        Dispose();
    }
}
