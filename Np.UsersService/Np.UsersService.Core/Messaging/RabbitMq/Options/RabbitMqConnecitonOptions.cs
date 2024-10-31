namespace Np.UsersService.Core.Messaging.RabbitMq.Options;

public class RabbitMqConnecitonOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
