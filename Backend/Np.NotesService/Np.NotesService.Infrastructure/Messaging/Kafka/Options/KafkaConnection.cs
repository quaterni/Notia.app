
namespace Np.NotesService.Infrastructure.Messaging.Kafka.Options;

/// <summary>
/// Kafka server connection options
/// </summary>
public class KafkaConnection
{
    /// <summary>
    /// Set Kafka's servers
    /// </summary>
    /// <value>
    /// <para>Servers of Kafka</para>
    /// </value>
    /// <example>
    /// <para>Delimeter: ","</para>
    /// In appsettings.json file :
    /// <code>
    /// "Servers": "localhost:9092,localhost:9093",
    /// </code>
    /// </example>
    public string Servers { get; set; } = string.Empty;
}
