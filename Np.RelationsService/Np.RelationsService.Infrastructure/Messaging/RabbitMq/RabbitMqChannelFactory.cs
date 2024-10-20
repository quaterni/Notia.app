
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;
using RabbitMQ.Client;

namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq;

internal partial class RabbitMqChannelFactory : IDisposable
{
    [LoggerMessage(Level =LogLevel.Information, Message ="Connection created: {Host}:{Port}")]
    private static partial void LogConnectionCreated(ILogger logger, string host, int port);

    [LoggerMessage(Level = LogLevel.Information, Message = "Connection created, channel number: {ChannelNumber}")]
    private static partial void LogChannelCreated(ILogger logger, int channelNumber);

    [LoggerMessage(Level = LogLevel.Information, Message = "Channel factory disposed")]
    private static partial void LogChannelFactoryDisposed(ILogger logger);

    private readonly ExchangeOptions _exchangeOptions;

    private bool _disposed;

    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqChannelFactory> _logger;

    public RabbitMqChannelFactory(
        ILogger<RabbitMqChannelFactory> logger,
        IOptions<ConnectionOptions> connectionOptions,
        IOptions<ExchangeOptions> exchangeOptions)
    {
        _exchangeOptions = exchangeOptions.Value;

        _connection = new ConnectionFactory()
        {
            HostName = connectionOptions.Value.HostName,
            Port = connectionOptions.Value.Port,
            UserName = connectionOptions.Value.UserName,
            Password = connectionOptions.Value.Password           
        }.CreateConnection();

        _connection.ConnectionShutdown += RabbitMqChannelFactory_ConnectionShutdown;
        _logger = logger;

        LogConnectionCreated(
            _logger,
            connectionOptions.Value.HostName,
            connectionOptions.Value.Port);
    }

    private void RabbitMqChannelFactory_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if(_disposed) return;
        Dispose();
    }

    public IModel Create()
    {
        if (_disposed) 
            throw new ApplicationException("Connection closed");
        var channel = _connection.CreateModel();
        channel.ExchangeDeclare(
            exchange: _exchangeOptions.Name, 
            type: _exchangeOptions.Type, 
            durable: _exchangeOptions.Durable, 
            autoDelete: _exchangeOptions.AutoDelete, 
            arguments: null);

        LogChannelCreated(_logger, channel.ChannelNumber);

        return channel;
    }

    public void Dispose()
    {
        if( _disposed) return;
        LogChannelFactoryDisposed(_logger);
        _connection.Dispose();
    }
}
