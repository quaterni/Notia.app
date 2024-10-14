
namespace Np.NotesService.Application.Abstractions.Outbox;

public class OutboxEntry
{
    public required Guid Id { get; init; }

    public required DateTime Created { get; init; }

    public required DateTime RefreshTime { get; set; }

    public required string EventName { get; init; }

    public required string Data { get; init; }
}
