
namespace Np.RelationsService.Application.Abstractions.Messaging.Events;

public record MessageBusEvent(string EventName, string Body);
