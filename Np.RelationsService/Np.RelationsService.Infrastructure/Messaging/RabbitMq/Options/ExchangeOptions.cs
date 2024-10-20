
namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;

internal class ExchangeOptions
{
    public string Name { get; init; }
    public string Type { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
}
