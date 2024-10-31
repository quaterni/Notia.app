namespace Np.UsersService.Core.Messaging.Outbox.Models;

public class OutboxEntry
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    public DateTime RefreshTime { get; set; }

    public DateTime Created { get; set; }
}
