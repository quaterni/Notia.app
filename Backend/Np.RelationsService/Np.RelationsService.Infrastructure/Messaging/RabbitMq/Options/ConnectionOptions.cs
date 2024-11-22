
namespace Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;

internal class ConnectionOptions
{
    public string HostName { get; init; }
    public int Port { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
}
