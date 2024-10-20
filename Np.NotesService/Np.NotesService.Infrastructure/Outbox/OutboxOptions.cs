
namespace Np.NotesService.Infrastructure.Outbox;

internal class OutboxOptions
{
    public required int EntryLimitPerCheck { get; init; }
    public required int CheckDelayMilliseconds { get; init; }
    public required int EntryRefreshTimeMinutes { get; init; }
}
