
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

internal class RabbitMqChannelFactory : IDisposable, IRabbitMqChannelFactory
{
    private readonly IConnection _connection;

    private readonly RabbitMqExchangeOptions _rabbitMqExchangeOptions;

    public RabbitMqChannelFactory(
        IOptions<RabbitMqConnectionOptions> connectionOptions,
        IOptions<RabbitMqExchangeOptions> exchangeOptions)
    {
        var opt = connectionOptions.Value;

        _rabbitMqExchangeOptions = exchangeOptions.Value;
        _connection = new ConnectionFactory() 
        {
            HostName = opt.HostName,
            Port = opt.Port,
            UserName = opt.User,
            Password = opt.Password
        }.CreateConnection();
    }

    public IModel CreateChannel()
    {
        var model = _connection.CreateModel();

        model.ExchangeDeclare(
            exchange: _rabbitMqExchangeOptions.ExchangeName, 
            type: _rabbitMqExchangeOptions.Type, 
            durable: _rabbitMqExchangeOptions.Durable, 
            autoDelete: _rabbitMqExchangeOptions.AutoDelete, 
            arguments: _rabbitMqExchangeOptions.Arguments);

        return model;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
