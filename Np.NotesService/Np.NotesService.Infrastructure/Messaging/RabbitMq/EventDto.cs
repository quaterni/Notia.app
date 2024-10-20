
namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

public class EventDto
{
    public required string EventName { get; init; }
    public required string Body { get; init; }
}
