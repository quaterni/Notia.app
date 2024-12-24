
namespace Np.NotesService.Infrastructure.Messaging.Kafka.Options;

internal class BaseProducerOptions : IProducerOptions
{
    public string Topic { get; set; } = string.Empty;
}
