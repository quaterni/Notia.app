using Np.RelationsService.Domain.Notes;

namespace Np.RelationsService.Domain.RootEntries
{
    public interface IRootEntriesRepository
    {
        Task<bool> IsEntryRoot(Note note, CancellationToken cancellationToken = default);

        Task<RootEntry?> GetRootEntryById(Guid rootEntryId);

        Task<RootEntry?> GetRootEntryByNoteId(Guid noteId);

        void Add(RootEntry rootEntry);

        void Remove(RootEntry rootEntry);
    }
}
