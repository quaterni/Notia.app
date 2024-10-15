
namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

internal class RabbitMqExchangeOptions
{
    public required string ExchangeName { get; set; }
    public required string Type { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public IDictionary<string, object>? Arguments { get; init; } = null;
}
