
using RabbitMQ.Client;

namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq;

internal class RabbitMqChannelFactory : IDisposable
{
    private bool _disposed;

    private readonly IConnection _connection;

    public RabbitMqChannelFactory()
    {
        _connection = new ConnectionFactory()
        {
            HostName = "messagebus"
        }.CreateConnection();

        _connection.ConnectionShutdown += RabbitMqChannelFactory_ConnectionShutdown;
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
            exchange: "events", 
            ExchangeType.Fanout, 
            durable: true, 
            autoDelete: false, 
            arguments: null);

        return channel;
    }

    public void Dispose()
    {
        if( _disposed) return;
        _connection.Dispose();
    }
}
