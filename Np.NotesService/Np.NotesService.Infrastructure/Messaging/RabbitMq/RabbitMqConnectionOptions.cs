
namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

internal class RabbitMqConnectionOptions
{
    public required string HostName { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
    public required int Port { get; init; }
}
