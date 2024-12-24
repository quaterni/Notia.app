
namespace Np.NotesService.Infrastructure.Messaging.Kafka.Options;

/// <summary>
/// Kafka producer options
/// </summary>
public interface IProducerOptions
{
    /// <summary>
    /// Name of Kafka's topic
    /// </summary>
    public string Topic { get; }
}
