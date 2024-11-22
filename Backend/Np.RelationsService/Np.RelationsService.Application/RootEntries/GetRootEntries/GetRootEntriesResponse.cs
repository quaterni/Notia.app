namespace Np.RelationsService.Application.RootEntries.GetRootEntries;

public sealed record GetRootEntriesResponse(IEnumerable<Guid> NoteIds);