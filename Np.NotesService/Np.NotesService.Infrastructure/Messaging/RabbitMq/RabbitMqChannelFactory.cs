
using RabbitMQ.Client;

namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

internal class RabbitMqChannelFactory : IDisposable, IRabbitMqChannelFactory
{
    private readonly IConnection _connection;

    public RabbitMqChannelFactory()
    {
        _connection = new ConnectionFactory() 
        {
            HostName = "messagebus"
        }.CreateConnection();
    }

    public IModel CreateChannel()
    {
        var model = _connection.CreateModel();

        model.ExchangeDeclare(
            exchange: "events", 
            type: ExchangeType.Fanout, 
            durable: true, 
            autoDelete: false, 
            arguments: null);

        return model;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
