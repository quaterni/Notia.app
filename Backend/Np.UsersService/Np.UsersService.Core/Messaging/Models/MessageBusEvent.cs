namespace Np.UsersService.Core.Messaging.Models;

public sealed record MessageBusEvent(string EventName, string Body);
