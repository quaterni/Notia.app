namespace Np.UsersService.Core.Messaging.RabbitMq.Options;

public class RabbitMqExchangeOptions
{
    public string ExchangeName { get; set; }
    public string Type { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
}
