
namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;

internal class QueueOptions
{
    public string Name { get; init; }
    public bool Durable { get; init; }
    public bool Exclusive { get; init; }
    public bool AutoDelete { get; init; }
}
