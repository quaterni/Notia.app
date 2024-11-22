namespace Np.UsersService.Core.Messaging.RabbitMq.Options;

public class RabbitMqQueueOptions
{
    public string QueueName { get; set; } = string.Empty;

    public bool Durable { get; set; }

    public bool AutoDelete { get; set; }

    public bool Exclusive { get; set; }

    public string ExchangeName { get; set; } = string.Empty;
}
