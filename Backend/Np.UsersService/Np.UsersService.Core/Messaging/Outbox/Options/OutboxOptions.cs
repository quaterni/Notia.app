namespace Np.UsersService.Core.Messaging.Outbox.Options;

public class OutboxOptions
{
    public int CheckLimit { get; set; }

    public int CheckTimeoutInSeconds { get; set; }

    public int RefreshAdditionInMinutes { get; set; }
}
